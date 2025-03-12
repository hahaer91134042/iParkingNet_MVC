using DevLibs;
using Fleck;
using GeoTimeZone;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// ObjExtend 的摘要描述
/// </summary>
public static class AppExtend
{
    #region BaseProcess
    public static Task runAsync(this BaseProcess process)
        => Task.Run(() => process.run());
    #endregion
    #region AddressRequest
    public static LatLng convertToLatLng(this AddressRequest address)
    {
        try
        {
            var connect = WebConnect.CreatGet(GoogleApi.Parse(address).ApiUrl);
            var result = connect.Connect<GoogleMapResult>();
            //result.saveLog("google api result");

            if (!result.isSuccess())
                throw new GoogleApiException();

            return new LatLng(
                result.results[0].geometry.location.lat,
                result.results[0].geometry.location.lng);
        }
        catch (Exception)
        {
            throw new GoogleApiException();
        }
    }
    #endregion

    #region IPhoneMap
    public static string getSmsCode(this IPhoneMap input)
    {
        return input.countryCode().Equals("886") ? input.phone() : $"{input.countryCode()}{Convert.ToInt64(input.phone())}";
    }
    #endregion

    #region IFormDataCobtrol
    public static EkiPostImg getPostImg(this IFormDataControl c, string flag)
    {
        if (c.formFileContain(flag))
        {
            var postImg = new EkiPostImg(c.getPostFile(flag));
            if (FileUtil.checkFileUploadFormate(postImg.exten, FileUtil.AllowFileOption.Img) != FileUtil.Result.OK)
                throw new InputFormatException();
            return postImg;
        }
         //return getPostImg(HttpContext.Current.Request.Files[flag]);
        return null;
    }
    #endregion

    #region JwtAuthObject
    public static Member getMember(this JwtAuthObject obj) => new Member().Also(m => m.CreatByUniqueId(obj.user));
    #endregion
    #region Location
    public static double getRatingCount(this Location loc)
    {
        var ratings = (from r in EkiSql.ppyp.table<MemberRating>()
                       where r.LocationId == loc.Id
                       orderby r.cDate descending
                       select r).Take(ApiConfig.MaxRatingNum);
        double count = ratings.Count();
        if (count == 0)
            return 0;

        double sum = ratings.Select(r => r.Star).Sum();

        return Math.Round(sum / count,1,MidpointRounding.AwayFromZero);
    }
    public static void setAvailable(this Location loc, DateTime now)
    {

        var reservaList = (from reserva in EkiSql.ppyp.table<ReservedTime>()
                           where reserva.LocationId == loc.Id
                           where reserva.getEndTime() >= now
                           select reserva).toSafeList();

        var reservaForbiCheck = new ForbiddenCheck(reservaList);
        var forbiCheck = loc.ReservaConfig.OpenSet.getOpenCheck(now);
        var isValid = !reservaForbiCheck.IsForbidden(now) && forbiCheck.IsOpen(now);
        loc.Available = isValid ? AvailableStatus.Available : AvailableStatus.Unavailable;
    }
    
    #endregion
    #region ReservedTime
    public static ReservaTimeResponse toResponse(this ReservedTime reserved,EkiOrder order=null,bool isUser=false)
    {
        if (order != null)
        {
            switch (order.StatusEnum)
            {
                case OrderStatus.BeSettle:
                case OrderStatus.CheckOut:
                    var checkout = new EkiCheckOut().Also(c => c.CreatedByOrderId(order.Id));
                    if (checkout.Id > 0)//表示有資料
                    {
                        return reserved.convertToResponse().setIsUser(isUser).Also(res =>
                        {
                            res.EndTime = checkout.Date.standarCheckOutTime(reserved).toString();
                        });
                    }
                    break;
            }
        }

        return reserved.convertToResponse().setIsUser(isUser);
    }
    public static void setEndTime(this ReservedTime reserved, DateTime time)
    {
        reserved.EndTime = time;
    }
    public static ReservedTime getMinTime(this List<ReservedTime> list)
    {
        return (from t in list
                orderby t.getStartTime() ascending
                select t).First();
    }

    public static DateTime getStartTime(this ReservedTime input)
    {
        //return TimeUtil.CombineToDateTime(input.Date, input.StartTime);
        return input.StartTime;
    }
    public static DateTime getEndTime(this ReservedTime input)
    {
        //return TimeUtil.CombineToDateTime(input.Date, input.EndTime);
        return input.EndTime;
    }
    //public static string getStartTimeStr(this ReservedTime input)
    //{
    //    return TimeUtil.CombineToTimeStr(input.Date, input.StartTime);
    //}
    //public static string getEndTimeStr(this ReservedTime input)
    //{
    //    return TimeUtil.CombineToTimeStr(input.Date, input.EndTime);
    //}
    #endregion
    #region OpenTime
    public static OpenCheck getOpenCheck(this List<OpenTime> list, DateTime now)
    {
        //找出非循環的
        var openList = (from f in list
                        where f.weekEnum == WeekEnum.NONE
                        where f.getEndTime() >= now
                        select f).toSafeList();
        //比對循環週期的Forbi 的Week
        openList.AddRange((from f in list
                           where f.weekEnum != WeekEnum.NONE
                           where f.Week == now.DayOfWeek.toInt()
                           select new OpenTime()
                           {
                               Date = now.ToString("yyyy-MM-dd"),
                               StartTime = f.StartTime,
                               EndTime = f.EndTime
                           }).toSafeList());

        return new OpenCheck(openList);
    }

    #endregion
    #region EkiOrder
    //這是用來判斷該訂單是否可以下載所使用
    public static bool checkLoadValid(this EkiOrder order)
    {
        //取消單不要
        //if (order.StatusEnum == OrderStatus.Cancel || order.StatusEnum == OrderStatus.CancelByManager)
        //    return false;

        //因為預約單可能還沒checkout所以要丟出
        if (order.StatusEnum == OrderStatus.Reserved)
            return true;
        //未結帳的單要丟出
        if (order.StatusEnum == OrderStatus.BeSettle)
            return true;

        var gap = DateTime.Now - order.cDate;

        return gap.TotalDays < ApiConfig.OrderExistDays;

        /*
        order.cDate.toDateTime(), ApiConfig.OrderExistDays
        var gap = DateTime.Now - time;
        return gap.TotalDays < day;
         */
    }
    //地主取消產生的罰金
    public static InvoiceResponse getInvoiceResponse(this EkiOrder order)
    {
        var invoiceNum = (from i in EkiSql.ppyp.table<InvoiceReturn>()
                       where i.OrderId == order.Id
                       select i.InvoiceNumber).FirstOrDefault();
        var card = (from p in EkiSql.ppyp.table<NewebPayReturn>()
                   where p.OrderId == order.Id
                   select p.Card4No).FirstOrDefault();
        return new InvoiceResponse
        {
            Number=invoiceNum.isNullOrEmpty()?"":invoiceNum,
            Card4No=card.isNullOrEmpty()?"":card
        };
    }
    public static double calManagerMulctAmt(this EkiOrder order, DateTime from)
    {
        var span = order.ReservaTime.getStartTime() - from;
        var rule = ApiConfig.ManagerMulctSet.CancelRules.FirstOrDefault(r => r.isInRule(span));
        rule.isNull(() => { throw new NoSuchRuleException(); });
        return (order.Cost.toDouble() * rule.mulctRatio()).toCurrency(order);
    }
    //地主取消之後對車主的補償
    public static double calCompensation(this EkiOrder order,DateTime from)
    {
        var span = order.ReservaTime.getStartTime() - from;
        var rule = ApiConfig.ManagerMulctSet.CancelRules.FirstOrDefault(r => r.isInRule(span));
        rule.isNull(() => { throw new NoSuchRuleException(); });
        return (order.Cost.toDouble() * rule.compensationRatio()) + rule.addtionalCompensation(order);
    }

    public static object toManagerLocOrder(this IEnumerable<EkiOrderResponse> list, Func<EkiOrderResponse, String> imgBack)
    {
        return (from order in list
                select new
                {
                    order.SerialNumber,
                    order.Cost,
                    order.Unit,
                    order.LocPrice,
                    order.LocSerial,
                    order.HandlingFee,
                    order.Status,
                    order.CarNum,
                    order.CreatTime,
                    //order.CheckOutUrl,
                    order.Rating,
                    order.Argue,
                    CheckOutImg = imgBack(order),
                    //CheckOutImg = checkOut.isNullOrEmpty()?"":checkOut.mapImgWith(DirPath.Order,member),
                    
                    order.Member,
                    order.ReservaTime,
                    order.Address,
                    order.Checkout
                });
    }
    public static DateTime standarOrderReservaTime(this DateTime input)
    {
        var start = new DateTime(input.Year, input.Month, input.Day, input.Hour,0,0);

        while (start < input)
        {
            start=start.AddMinutes(ApiConfig.BillingMinOffsetMinute);
        }

        return start;
    }

    //標準化規則是 30分鐘以內 以30分鐘計算 超過30分 每15分計算一次
    public static DateTime standarCheckOutTime(this DateTime time, ReservedTime reservedTime)
    {
        var checkoutTime = time;
        //30分鐘以內 checkout
        var rStart = reservedTime.getStartTime();
        var rEnd = reservedTime.getEndTime();
       

        if ((checkoutTime - rStart).TotalMinutes <= ApiConfig.MinBillingMinute)
            return rStart.AddMinutes(ApiConfig.MinBillingMinute);

        var checkOutTotalSec = (checkoutTime - rStart).TotalSeconds;
        var freeSec = ApiConfig.FreeCheckoutMinute * 60;

        //假如結算時間只有超出正常時間一點(現在是10分鐘以內)的時候 以正常時間計算
        //該免費時段只能套用一次而已
        if (checkOutTotalSec<((rEnd-rStart).TotalSeconds+ freeSec))
        {
            //計算是不是有Free的時段 假如有Free時段 去掉
            var freeSpanSec = checkOutTotalSec % (ApiConfig.BillingMinOffsetMinute * 60);

            if (freeSpanSec <=freeSec)            
                checkoutTime = time.AddSeconds(-freeSpanSec);
        }
       

        var standarList = new List<DateTime>();
        var start = new DateTime(checkoutTime.Year, checkoutTime.Month, checkoutTime.Day, checkoutTime.Hour, 0, 0);
        var end = start.AddHours(1);
        do
        {
            standarList.Add(start);
            start = start.AddMinutes(ApiConfig.BillingMinOffsetMinute);
        } while (start <= end);

        foreach (var standar in standarList)
        {
            if (checkoutTime <= standar)
                return standar;
        }
        return checkoutTime;
    }

    public static string getPayUrl(this EkiOrder order)
    {
        return $"{WebUtil.getWebURL()}{string.Format(PayConfig.PayPagePath, order.UniqueID.toString())}";
    }
    public static string getPayReturnUrl(this EkiOrder order)
    {
        return PayConfig.藍新.config().returnUrl(order.UniqueID.toString());
    }
    public static string getPayNotifyUrl(this EkiOrder order)
    {
        return PayConfig.藍新.config().notifyUrl(order.UniqueID.toString());
    }
    public static bool isNoPayOrder(this EkiOrder order)
    {
        switch (order.StatusEnum)
        {
            case OrderStatus.BeSettle:
                return true;
            case OrderStatus.Reserved:
                //這是表示已經超過時間但是還沒結帳的單
                var end = order.ReservaTime.getEndTime();
                return (DateTime.Now - end).TotalSeconds > 0;
            case OrderStatus.Cancel:
                return new OrderCancel().Also(o => o.CreatById(order.Id)).Paid;
        }

        return false;
    }
    public static bool isReserva(this EkiOrder order, DateTime now)
    {
        if (order.StatusEnum == OrderStatus.Reserved)
        {
            var start = order.ReservaTime.getStartTime();
            return start > now;
        }
        return false;
    }
    public static bool isCancelable(this EkiOrder order, DateTime now)
    {
        var min = order.spanStartMin(now);
        return min >= 0;
        //return min >= ApiConfig.OrderCancelMin;
    }
    public static double spanStartMin(this EkiOrder order, DateTime now)
    {
        var startTime = order.ReservaTime.getStartTime();
        return (startTime - now).TotalMinutes;
    }
    #endregion
    #region VehicleInfo
    public static void updateVehicleDefault(this VehicleInfo info)
    {
        //因為default只能一個為true
        if (info.IsDefault)
        {
            var pair = QueryPair.New().addQuery("MemberId", info.MemberId);
            var list = (from v in EkiSql.ppyp.tableByPair<VehicleInfo>(pair)
                        where v.Id != info.Id
                        where v.IsDefault
                        select v).toSafeList();
            list.ForEach(v =>
            {
                v.IsDefault = false;
                v.Update();
            });
        }
    }
    #endregion
    #region ITimeMap
    public static bool between(this ITimeRange range,DateTime time)
    {
        return range.start() <= time && time <= range.end();
    }
    public static bool between(this ITimeMap map,DateTime time)
    {
        //開放週期性相同
        if (map.week() == time.DayOfWeek.toInt())
        {

            var newOpen = new OpenTime()
            {
                Date = time.ToString(ApiConfig.DateFormat),
                StartTime = map.startTime(),
                EndTime = map.endTime()
            };
            return new OpenSet(newOpen.getStartTime(), newOpen.getEndTime()).IsBetween(time);
        }
        return new OpenSet(map.getStartTime(), map.getEndTime()).IsBetween(time);
    }
    public static bool equals(this ITimeMap ori,ITimeMap other)
    {
        if (ori.week() == other.week())
            return ori.getStartTime().toStamp() == other.getStartTime().toStamp() &&
                ori.getEndTime().toStamp() == other.getEndTime().toStamp();
        return false;
    }
    public static DateTime getStartTime(this ITimeMap input)
    {
        if (input.week() != WeekEnum.NONE.toInt())
        {
            var now = DateTime.Now;
            var nowWeek = now.DayOfWeek.toInt();
            var date = now.AddDays(input.week() - nowWeek).ToString(ApiConfig.DateFormat);
            return TimeUtil.CombineToDateTime(date, input.startTime());
        }
        return TimeUtil.CombineToDateTime(input.date(), input.startTime());
    }
    public static DateTime getStartTime(this ITimeMap input,DateTime now)
    {
        if (input.week() != WeekEnum.NONE.toInt())
        {
            var nowWeek = now.DayOfWeek.toInt();
            var date = now.AddDays(input.week() - nowWeek).ToString(ApiConfig.DateFormat);
            return TimeUtil.CombineToDateTime(date, input.startTime());
        }
        return TimeUtil.CombineToDateTime(input.date(), input.startTime());
    }
    public static TimeSpan getStartSpan(this ITimeMap map) => map.startTime().toTimeSpan();
    public static TimeSpan getEndSpan(this ITimeMap map) => map.endTime().toTimeSpan();

    public static bool timeSpanBetween(this ITimeMap map, TimeSpan other)
    {
        var start = map.getStartSpan();
        var end = map.getEndSpan();
        return start <= other && other <= end;
    }
    public static DateTime getEndTime(this ITimeMap input)
    {
        if (input.week() != WeekEnum.NONE.toInt())
        {
            var now = DateTime.Now;
            var nowWeek = now.DayOfWeek.toInt();
            var date = now.AddDays(input.week() - nowWeek).ToString(ApiConfig.DateFormat);
            return TimeUtil.CombineToDateTime(date, input.endTime());
        }
        return TimeUtil.CombineToDateTime(input.date(), input.endTime());
    }
    public static DateTime getEndTime(this ITimeMap input,DateTime now)
    {
        if (input.week() != WeekEnum.NONE.toInt())
        {
            var nowWeek = now.DayOfWeek.toInt();
            var date = now.AddDays(input.week() - nowWeek).ToString(ApiConfig.DateFormat);
            return TimeUtil.CombineToDateTime(date, input.endTime());
        }
        return TimeUtil.CombineToDateTime(input.date(), input.endTime());
    }
    public static string getStartTimeStr(this ITimeMap input)
    {
        return TimeUtil.CombineToTimeStr(input.date(), input.startTime());
    }
    public static string getEndTimeStr(this ITimeMap input)
    {
        return TimeUtil.CombineToTimeStr(input.date(), input.endTime());
    }
    #endregion
    #region IMapImg
    public static void deleteImgWith(this IMapImg input,Member member)
    {
        var fileInfo = new FileInfo($"{DirPath.Member}/{member.UniqueID.toString()}/{input.imgName()}".toServerPath());
        if (fileInfo.Exists)
            fileInfo.Delete();
    }
    public static string mapMemberImgUrl(this IMapImg input, Member member)
    {
        //return $"{WebUtil.getWebURL()}{DirPath.Member}/{member.UniqueID}/{input.imgName()}";
        return input.mapImgUrlWith(DirPath.MemberImg, member);
    }    
    public static string mapImgUrlWith(this IMapImg input, string dirPath, Member member)
    {
        if (!input.imgName().isNullOrEmpty())
            return $"{string.Format(dirPath, member.UniqueID.toString())}/{input.imgName()}".toLinkUrl();
        return "";
    }
    public static string mapImgServerPath(this IMapImg input,string dirPath,Member member)
    {
        if (!input.imgName().isNullOrEmpty())
            return $"{string.Format(dirPath, member.UniqueID.toString())}/{input.imgName()}".toServerPath();
        return "";
    }
    #endregion
    #region IBitmapImg
    public static void saveBitmapWith(this IBitmapImg input, Member member, long quality = 30L)
        => input.saveBitmap($"{DirPath.Member}/{member.UniqueID.toString()}".toServerPath(),quality);
    public static void saveOrderBitmapWith(this IBitmapImg input, Member member, long quality = 30L)
        => input.saveBitmap(string.Format(DirPath.Order, member.UniqueID.toString()).toServerPath(),quality);

    public static void saveBitmap(this IBitmapImg input,string dirPath,long quality=30L)
    {
        dirPath.creatDir();//保險起見
        if (input.bitmap() != null)
            ImgUtil.SaveBitmap(input.bitmap(), $"{dirPath}/{input.imgName()}", input.imgExten(),quality);
    }
    #endregion
    #region JwtAuthObject
    public static LocationOwnerManager creatLocationOwnerManager(this JwtAuthObject auth)
        => new LocationOwnerManager(auth.user);
    #endregion
    #region IPushSet
    public static void sendCancelOrderMsg(this Member member, EkiOrder order, Location loc)
    {
        member.sendPushMsg(new CancelOrderContent
        {
            Name = loc.Info.InfoContent,
            Start = order.ReservaTime.StartTime.toString(),
            End = order.ReservaTime.EndTime.toString(),
            CarNum = order.CarNum
        });
    }
    public static void sendGetOrderMsg(this Member member, EkiOrder order, Location loc)
    {
        member.sendPushMsg(new GetOrderContent
        {
            Name = loc.Info.InfoContent,
            Start = order.ReservaTime.StartTime.toString(),
            End = order.ReservaTime.EndTime.toString(),
            CarNum = order.CarNum
        });
    }
    public static void sendPushMsg(this IPushSet set,IBroadCastMsg msg)
    {
        var socket = BroadcastSocket.Instance;
        using (var fcmManager = FcmManager.New())
        {
            try
            {
                if (!socket.SendTo(set.socketID(), msg))
                {
                    fcmManager.SendTo(set.fcmToken(),set.device(), msg);
                }
            }
            catch (Exception e)
            {
                //e.saveLog("SendBroadCast", "send", msg);
            }
        }
    }
    #endregion
    #region BroadCastMsg
    public static void Send(this IWebSocketConnection socket,ISocketMsg msg)
    {
        socket.Send(msg.toSocketMsg().toJsonString());
    }
    public static SocketMsgShell toSocketMsg(this ISocketMsg msg) => new SocketMsgShell
    {
        Method = msg.socketMethod(),
        Content = msg
    };
    public static void sendTo(this IBroadCastMsg msg, IPushSet set)
    {
        var socket = BroadcastSocket.Instance;
        using (var fcmManager = FcmManager.New())
        {
            try
            {
                if (!socket.SendTo(set.socketID(), msg))
                {
                    fcmManager.SendTo(set.fcmToken(),set.device(), msg);
                }
            }
            catch (Exception e)
            {
                e.saveLog("SendBroadCast", "send", msg);
            }
        }
    }
    public static void sendTo(this IBroadCastMsg msg, IEnumerable<IPushSet> list)
    {
        //有socket先送 沒有再送Fcm 
        var socket = BroadcastSocket.Instance;
        using (var fcmManager = FcmManager.New())
        {
            var socketMsg = msg.toSocketMsg();
            foreach (var set in list)
            {
                try
                {
                    if (!socket.SendTo(set.socketID(),socketMsg.toJsonString()))
                    {
                        fcmManager.SendTo(set.fcmToken(),set.device(), msg);
                    }
                }
                catch (Exception e)
                {
                    e.saveLog("SendBroadCast", "sendMulti", msg);
                }
            }
        }
    }
    #endregion
    #region IPriceSet
    public static double toCurrency(this double cost, ICurrencySet set)
    {
        switch (set.currencyUnit())
        {
            case CurrencyUnit.USD:
                return cost;//有小數點的
            default:
                return cost.Round();//四捨五入
        }
    }
    public static decimal toCurrency(this decimal cost, ICurrencySet set)
    {
        switch (set.currencyUnit())
        {
            case CurrencyUnit.USD:
                return cost;//有小數點的
            default:
                return cost.Round();//四捨五入
        }
    }
    public static decimal getHourPrice(this IPriceSet<decimal> set) => set.price() * (60 / set.methodSet().Min);
    public static decimal getHalfHourPrice(this IPriceSet<decimal> set) => set.price() * (30/ set.methodSet().Min);
    #endregion

    #region IEncodeSet
    public static string creatHash(this IEncodeSet input, string publicKey)
        => input.creatHash(publicKey, input.hashSet().secret());
    public static string creatHash(this IEncodeSet input, params string[] array)
        => input.hashSet().creatHash(array.Count() > 0 ? array : new string[] { input.hashSet().secret() });
    public static string encryptByAES(this IEncodeSet input, string hashText = null)
    {
        if (hashText == null)
            hashText = input.creatHash();
        return SecurityBuilder.EncryptTextByAES(input.toJsonString(), hashText);
    }
    public static T decryptByAES<T>(this string input, string hashText = null) where T : IEncodeSet
    {
        if (hashText == null)
            hashText = Activator.CreateInstance<T>().creatHash();
        return SecurityBuilder.DecryptTextByAES(input, hashText).toObj<T>();
    }
    #endregion
    #region IToken
    public static string token(this IToken input)
        => JwtBuilder.GetEncoder()
                .setUser(input.tokenRaw())
                .setExpDate(DateTime.Now.AddDays(ApiConfig.TokenLifeDay))
                .encode();
    #endregion
    #region ICryptoFormat
    public static string creatHash(this ICryptoFormat input, params string[] valus) =>
        SecurityBuilder.CreateHashCode(input.format(), valus);
    #endregion
    #region ICryptoPwd
    public static void creatCipher(this ICryptoPwd input,string rawText)
    {
        var salt = input.newSalt();
        var cipher = input.creatHash(salt, rawText);
        input.setSalt(salt);
        input.setCipher(cipher);
    }
    public static bool checkCipher(this ICryptoPwd input, string rawText)
        => input.creatHash(input.salt(), rawText).Equals(input.cipher());
    #endregion
    #region ILinkId
    public static List<T> selectListById<T>(this ILinkId input)where T:BaseDbDAO
    {
        if (typeof(T) is ILinkId)
            return (from obj in EkiSql.ppyp.table<T>()
                    where (obj as ILinkId).linkId()==input.linkId()
                    select obj).toSafeList();
        return new List<T>();
    }
    #endregion
    #region DateTime

    public static DateTime toLocationTime(this DateTime org, Location loc) => org.toLocationTime(loc.Lat, loc.Lng);
    /// <summary>
    /// 時間轉換到特定經緯度時區 需下載GeoTimeZone,NodaTime包來使用
    /// </summary>
    /// <param name="org"></param>
    /// <param name="lat"></param>
    /// <param name="lng"></param>
    /// <returns></returns>
    public static DateTime toLocationTime(this DateTime org, double lat, double lng)
    {
        try
        {

            //var orgUTC = org.ToUniversalTime();
            //Log.d($"orgUTC->{orgUTC.toString()}");
            var instant = Instant.FromDateTimeUtc(org.ToUniversalTime());
            var zoneID = TimeZoneLookup.GetTimeZone(lat, lng).Result;
            //Log.d($"Time Zone Id->{zoneID}");
            var timeZone = DateTimeZoneProviders.Tzdb[zoneID];
            //Log.d($"time zone->{timeZone.toJsonString()}");
            var result = instant.InZone(timeZone);
            //var localId = $"{TimeZoneLookup.GetTimeZone(lat, lng).Result.Split('/')[1]} Standard Time";
            //Log.d($"zone convert result->{result}");
            //var timeZone = TimeZoneInfo.FindSystemTimeZoneById(localId);
            //return result.ToDateTimeUtc();
            return result.ToDateTimeUnspecified();
        }
        catch (Exception ex)
        {
            //Log.e("toLocationTime Error", ex);
        }
        return org;
    }

    //public static string toString(this DateTime time, string format = ApiConfig.DateTimeFormat) => time.ToString(format);
    #endregion

    public static bool equal(this Guid uid, string id) => uid.ToString().Equals(id, StringComparison.OrdinalIgnoreCase);
    public static bool equal(this string id, Guid uid) => uid.equal(id);
    public static void saveLog(this Exception ex, string msg, string api = "", object input = null)
    {
        new ErrorLog()
        {
            Api = api,
            Input = input?.toJsonString(),
            Exception = ex.ToString(),
            Msg = msg,
            Ip = WebUtil.GetUserIP()
        }.Insert();
    }
    public static void saveLog(this object obj, string msg, Exception ex = null)
    {
        new ErrorLog()
        {
            Api = "",
            Input = obj.toJsonString(),
            Exception = ex?.ToString(),
            Msg = msg,
            Ip = WebUtil.GetUserIP()
        }.Insert();
    }
    public static void saveLog(this object obj, string msg)
    {
        new ErrorLog()
        {
            Api = "",
            Input = obj.toJsonString(),
            Exception = "",
            Msg = msg,
            Ip = WebUtil.GetUserIP()
        }.Insert();
    }
}