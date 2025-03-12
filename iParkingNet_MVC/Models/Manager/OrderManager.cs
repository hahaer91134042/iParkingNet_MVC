using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using OCPP_1_6;
using Eki_OCPP;
using Eki_LinePayApi_v3;
using Eki_NewebPay;

/// <summary>
/// OrderManager 的摘要描述
/// </summary>
public class OrderManager : BaseManager, ISerialNumGenerator
{
    public Member member;//這邊是預約者的Member

    public OrderManager(string uniqueID)
    {
        member = new Member();        
        member.CreatByUniqueId(uniqueID);
        //Log.print($"OrderManager member->{member.toJsonString()}");
    }

    public NewebPayCreditReturn payCreditAgree(EkiOrder order)
    {
        if (order.MemberId != member.Id)
            throw new ArgumentException();

        var mPayInfo = member.PayInfo;
        //Log.print($"PayCreditAgree pay info->{mPayInfo.toJsonString()}  useable->{!(mPayInfo?.neweb?.useable() ?? false)}");
        if (!(mPayInfo?.neweb?.useable() ?? false))
            throw new ArgumentNullException();

        var info = getPayInfo(order);
        var orderNo = OrderPayRecord.NewebPay(order.Id).Also(record => record.Insert()).EkiOrderSerial;

        var creditModel = new NewebPayCreditModel
        {
            TimeStamp = info.newebStamp,
            MerchantOrderNo = orderNo,
            Amt = info.amt.toInt(),//目前沒有貨幣類別的換算
            ProdDesc = info.desc,
            PayerEmail = order.Member.Mail,
            TokenTerm = order.Member.PhoneNum,
            TokenValue = mPayInfo.neweb.Token
        };

        var resp = NewebPay.CreditCard().Post(creditModel);

        //Log.print($"Credit agree resp->{resp.toJsonString()}");

        NewebPayReturn.Load(resp).Also(rModel =>
         {
             rModel.OrderId = order.Id;
             rModel.Insert();

             new OrderCheckOutProcess(order)
             {
                 checkSuccess = () => resp.Status == "SUCCESS",
                 tradeNo = () => rModel.TradeNo,
                 card4No = () => rModel.Card4No
             }.run();

             mPayInfo.neweb.TokenLife = resp.Result.TokenLife;
             mPayInfo.Update();

         });

        return resp;
    }

    public async Task<object> linePayReserve(EkiOrder order)
    {        
        if (member.Id != order.MemberId)
            throw new ArgumentException();

        var payInfo = getPayInfo(order);


        var record = OrderPayRecord.LinePay(order.Id);

        var lineReserve = new LinePayReserve()
        {
            orderId = record.EkiOrderSerial,
            redirectUrls = new LinePayReserve.RedirectUrl
            {
                confirmUrl = EkiLinePay.Config.confirmUrl(order.SerialNumber),
                cancelUrl = EkiLinePay.Config.cancelUrl()
            },
            packages = new LinePayReserve.Packages()
            {
                new LinePayReserve.Package()
                {
                    products=new List<LinePayReserve.Product>
                    {
                        new LinePayReserve.Product()
                        {
                            id = order.SerialNumber,
                            name = payInfo.desc,
                            price = payInfo.amt.toInt()
                        }
                    }
                }
            }
        };

        var client = new LinePay.Client(EkiLinePay.Config);

        var result = await client.ReserveAsync(lineReserve);

        if (result.returnCode == LineCode.Request.Success)
        {
            var rid = record.Insert(true);

            new OrderLinePay
            {
                RecordId = rid,
                TransactionId = result.info.transactionId,
                ReserveResult = result.toJsonString()
            }.Insert();
        }


        return new
        {
            App=result.info.paymentUrl.app,
            Web=result.info.paymentUrl.web
        };
    }

    public object checkChargeStatus(List<string> orderSerNum)
    {
        try
        {
            //Log.print($"order ser->{orderSerNum.toJsonString()}");

            var now = DateTime.Now;

            var datas = (from s in orderSerNum
                         join o in EkiSql.ppyp.table<EkiOrder>() on s equals o.SerialNumber
                         join l in EkiSql.ppyp.table<Location>() on o.LocationId equals l.Id
                         where o.beEnable
                         where o.StatusEnum == OrderStatus.Reserved
                         where o.MemberId == member.Id
                         where o.between(now.toLocationTime(l))
                         where l.Cp != null
                         select new
                         {
                             Order = o,
                             Loc = l
                         }).toSafeList();

            //Log.print($"checkChargeStatus order->{datas.FirstOrDefault().Order.SerialNumber}  loc ser->{datas.FirstOrDefault().Loc.SerNum}");
            //Log.print($"cp ser->{datas.FirstOrDefault().Loc.Cp.CpSerial}");

            var result = (from s in orderSerNum                          

                          select datas.FirstOrDefault(d => d.Order.SerialNumber == s)
                          .Let(data =>
                          {
                              ChargePoint cp = null;

                              data.notNull(d =>
                              {
                                  cp = EkiOCPP.getCP(d.Loc.Cp.CpSerial);

                              });

                              //Log.print($"cp ser->{data.Loc.Cp.CpSerial}");                              
                              //Log.print($"cp -> serial->{cp?.serial}");

                              return new
                              {
                                  SerNum = s,
                                  //CP = cp==null?"":cp?.serial,
                                  CpStatus=cp==null?OCPP_Status.CP.Unavailable.ToString():cp?.status.ToString()
                              };
                          }));

            return result;
        }
        catch(Exception e)
        {
            //Log.e("checkChargeStatus", e);
        }
        return null;
    }

    public IEnumerable<object> startCharge(List<string> orderSerNum)
    {
        var now = DateTime.Now;

        var datas = (from s in orderSerNum
                     join o in EkiSql.ppyp.table<EkiOrder>() on s equals o.SerialNumber
                     join l in EkiSql.ppyp.table<Location>() on o.LocationId equals l.Id
                     where o.beEnable
                     where o.StatusEnum == OrderStatus.Reserved
                     where o.MemberId == member.Id
                     where o.between(now.toLocationTime(l))
                     where l.Cp != null
                     select new
                     {
                         Order = o,
                         Loc = l
                     }).toSafeList();


        var result = (from s in orderSerNum
                      select datas.FirstOrDefault(d => d.Order.SerialNumber == s)
                      .Let(data =>
                      {
                          data.notNull(d => new StartChargeProcess(d.Loc).runAsync());

                          return new
                          {
                              SerNum = s,
                              Success = data != null
                          };
                      }));

        //result.Foreach(r =>
        //{
            
        //});

        return result;
    }

    public List<EkiOrderResponse> LoadUserOrder(List<string> serList = null)
    {
        var searchAll = serList == null ? true : serList.Count > 0 ? false : true;

        // var location = GetTable<Location>();

        var orders = (from order in GetTable<EkiOrder>()
                      where order.beEnable
                      where order.MemberId == member.Id
                      where searchAll ? true : serList.Contains(order.SerialNumber)
                      where order.checkLoadValid()
                      // join loc in location on order.LocationId equals loc.Id
                      orderby order.Id descending
                      orderby order.cDate descending
                      select order.convertToResponse()
                      .setRating((from r in GetTable<MemberRating>()
                                  where r.OrderId == order.Id
                                  where r.MemberId == member.Id
                                  select r).Count() < 1)
                      .setArgue((from a in GetTable<Argue>()
                                 where a.OrderId == order.Id
                                 where a.Source.toEnum<ArgueSource>() == ArgueSource.CarUser
                                 where a.MemberId == member.Id
                                 select a).Count() < 1)).toSafeList();

        return orders;

        //var orderList = new DbObjList<EkiOrder>(orders);
        //return orderList.convertResponseList<EkiOrderResponse>();
    }

    //直接設定結束時間的話 只有該結束時間點是開放的 就可以使用
    public List<object> setOrderEnd(List<SearchRequest.ExtendOrderEnd> data)
    {

        //延展的時間最小必須大於15分鐘
        var extOrders = (from ext in data
                         join o in GetTable<EkiOrder>() on ext.serNum equals o.SerialNumber
                         where o.beEnable
                         where o.MemberId == member.Id
                         where o.StatusEnum == OrderStatus.Reserved
                         where (ext.time.toDateTime() - o.ReservaTime.getEndTime()).TotalMinutes >= ApiConfig.BillingMinOffsetMinute

                         join loc in GetTable<Location>() on o.LocationId equals loc.Id
                         where validReservaTime_v2(loc, new ReservedTime()
                         {
                             StartTime = o.ReservaTime.StartTime,
                             EndTime = ext.time.toDateTime()
                         })
                         select o.Also(order =>
                         {
                             //更改預約時間
                             var extEnd = ext.time.toDateTime().standarOrderReservaTime();
                             order.ReservaTime.setEndTime(extEnd);
                             //更改價格
                             order.Cost = OrderCalculater.calOrderCost(order.ReservaTime, order);
                         })).toSafeList();

        //if (extOrders.Count() < 1)
        //    throw new ArgumentNullException();
        //extOrders.saveLog("update ExtOrders ");

        //Log.print($"exten orders->{extOrders.toJsonString()}");

        var tList = new DbObjList<EkiOrder>(extOrders);

        var uSuccess = tList.UpdateByObj();
        //Log.print($"extend update->{uSuccess}");

        //uSuccess.saveLog("update ext order successful");
        //if (!tList.UpdateByObj())
        //    throw new ArgumentException();

        var loadManager = new LoadManager();
        var result = (from ext in data
                      join o in GetTable<EkiOrder>() on ext.serNum equals o.SerialNumber
                      where o.beEnable
                      where o.MemberId == member.Id
                      where o.StatusEnum == OrderStatus.Reserved
                      join loc in GetTable<Location>() on o.LocationId equals loc.Id
                      select tList.Any(o => o.SerialNumber == ext.serNum).Let<bool, object>(success =>
                      {
                          EkiOrder order = null;
                          if (success)
                          {
                              order = tList.FirstOrDefault(o => o.SerialNumber == ext.serNum);
                              ////發送延長成功的推撥
                              //OrderExtendContent.load(order)
                              //.sendTo(loc.Member);
                          }

                          return new
                          {
                              Serial = ext.serNum,
                              Order = order != null ? order?.convertToResponse() : null,
                              ReservaStatus = success ? null : loadManager.getLocReservaTime(
                                    new List<Location> { loc },
                                    DateTime.Now.toLocationTime(loc),
                                    member).FirstOrDefault()
                          };
                      })).toSafeList();


        return result;

    }

    //這邊的展延只能展延當日的
    public List<object> extendOrders(List<SearchRequest.ExtendOrderEnd> data)
    {
        //延展的時間最小必須大於15分鐘
        var extOrders = (from ext in (from d in data
                                                      join o in GetTable<EkiOrder>() on d.serNum equals o.SerialNumber
                                                      select new { Order = o, Time = d.time.toDateTime() })
                         where ext.Order.beEnable
                         where ext.Order.MemberId == member.Id
                         where ext.Order.StatusEnum == OrderStatus.Reserved
                         //join loc in GetTable<Location>() on extOrder.LocationId equals loc.Id
                         where (ext.Time - ext.Order.ReservaTime.getEndTime()).TotalMinutes >= ApiConfig.BillingMinOffsetMinute
                         where validReservaTime_v2(
                             ext.Order.Location,
                             new ReservedTime
                             {
                                 StartTime = ext.Order.ReservaTime.StartTime,
                                 EndTime = ext.Time
                             },
                             (from lo in GetTable<EkiOrder>()
                              where lo.LocationId == ext.Order.LocationId
                              where lo.beEnable
                              where lo.Status < OrderStatus.InUsing.toInt()
                              //因為是要延展 所以已經有的先去掉在計算一次
                              where lo.Id != ext.Order.Id
                              //縮小範圍(要跨日預約延展時間所以之後的日期也要包含)
                              where lo.getStartTime().Date >= ext.Order.getStartTime().Date
                              select lo.ReservaTime))
                         /*
                          (from r in GetTable<ReservedTime>()
                              where !r.IsCancel
                              where r.LocationId == loc.Id
                              //因為是要延展 所以已經有的先去掉在計算一次
                              where r.Id != o.ReservaTime.Id

                              //縮小範圍(要跨日預約延展時間所以之後的日期也要包含)
                              where r.getStartTime().Date == o.ReservaTime.getStartTime().Date

                              //因為訂單假如已經結帳 要改用訂單的結帳時間(預約時間結帳之後不會再變更)
                              join ro in GetTable<EkiOrder>() on r.Id equals ro.ReservedId
                              select new ReservedTime
                              {
                                  Week = r.Week,
                                  StartTime = r.StartTime,
                                  EndTime = ro.getEndTime()
                              })
                          */
                         select ext.Order.Also(order =>
                         {
                             //更改預約時間
                             var extEnd = ext.Time.standarOrderReservaTime();
                             order.ReservaTime.setEndTime(extEnd);
                             //更改價格
                             order.Cost = OrderCalculater.calOrderCost(order.ReservaTime, order);
                         })).toSafeList();

        //if (extOrders.Count() < 1)
        //    throw new ArgumentNullException();
        //extOrders.saveLog("update ExtOrders ");

        //Log.print($"exten orders->{extOrders.toJsonString()}");

        var tList = new DbObjList<EkiOrder>(extOrders);

        var uSuccess= tList.UpdateByObj();
        //Log.print($"extend update->{uSuccess}");
        //uSuccess.saveLog("update ext order successful");
        //if (!tList.UpdateByObj())
        //    throw new ArgumentException();

        var loadManager = new LoadManager();
        var result = (from ext in data
                      join o in GetTable<EkiOrder>() on ext.serNum equals o.SerialNumber
                      where o.beEnable
                      where o.MemberId == member.Id
                      where o.StatusEnum == OrderStatus.Reserved
                      join loc in GetTable<Location>() on o.LocationId equals loc.Id
                      select tList.Any(o => o.SerialNumber == ext.serNum).Let<bool,object>(success =>
                      {
                          EkiOrder order = null;
                          if (success)
                          {
                              order = tList.FirstOrDefault(o => o.SerialNumber == ext.serNum);
                              //發送延長成功的推撥
                              OrderExtendContent.load(order)
                              .sendTo(loc.Member);
                          }

                          return new
                          {
                              Serial = ext.serNum,
                              Order = order != null ? order?.convertToResponse() : null,
                              ReservaStatus = success ? null : loadManager.getLocReservaTime(
                                    new List<Location> { loc },
                                    DateTime.Now.toLocationTime(loc),
                                    member).FirstOrDefault()
                          };
                      })).toSafeList();


        return result;
    }

    public List<EkiOrder> CreatReservaOrder(DbObjList<ReservedTime> reservaList)//多個位置預約
    {
        //var orders = (from o in GetTable<EkiOrder>()
        //              where reservaList.Any(r=>r.LocationId==o.LocationId)
        //              select o);
        using (var checkProcess = new CheckProcess())
        {
            var userOrder = (from order in GetTable<EkiOrder>()
                            where order.StatusEnum==OrderStatus.Reserved||order.StatusEnum==OrderStatus.InUsing
                             where order.MemberId == member.Id
                             select order);
            checkProcess
                .add(new OrderNoPayCheck(userOrder))
                .add(new OrderSameTimeCheck(userOrder,reservaList))
                .run();
        }

        var now = DateTime.Now;

        var locations = (from res in reservaList
                         join loc in GetTable<Location>() on res.LocationId equals loc.Id
                         select loc);//先找出使用者要預約的位置資料
        //找出該位置所有的預約單
        var locOrders = (from loc in locations
                         join order in (from o in GetTable<EkiOrder>()
                                        where o.beEnable
                                        //找出地點底下訂單的預約是正在使用的
                                        where (o.StatusEnum == OrderStatus.Reserved || o.StatusEnum == OrderStatus.InUsing)
                                        select o) on loc.Id equals order.LocationId into LocOrders

                         select new
                         {
                             Loc = loc,
                             Orders = (from o in LocOrders
                                       where IsValidReservaDay(o, now.toLocationTime(loc))
                                       select o)
                         });

        //var reservations = (from order in locOrders
        //                     //找出位置的已經預約時間跟正在使用
        //                    where (order.StatusEnum == OrderStatus.Reserved || order.StatusEnum == OrderStatus.InUsing)
        //                    where IsValidReservaDay(order)
        //                    select order.ReservaTime);
        //reservations.saveLog("已經預約訂單 ");
        //***注意 亂用join會可能導致資料多出來
        //選出可以預約的時間
        var goReservaTime = (from res in reservaList
                             join locOrder in locOrders on res.LocationId equals locOrder.Loc.Id
                             where validReservaTime_v2(
                                 locOrder.Loc,
                                 res,
                                 (from o in locOrder.Orders                                  
                                  select o.ReservaTime))

                             //where validReservaTime((from loc in locations
                             //                        where loc.Id == res.LocationId
                             //                        select loc).First(),
                             //                        (from reserved in reservations
                             //                         where reserved.LocationId == res.LocationId
                             //                         select reserved), res)

                             select new ReservedTime()
                             {
                                 LocationId = res.LocationId,
                                 MemberId = member.Id,
                                 //Date = res.Date,
                                 StartTime = res.StartTime,
                                 EndTime = res.EndTime,
                                 CarNum = res.CarNum,
                                 Remark = res.Remark
                             }).toSafeList();
        //goReservaTime.saveLog("goReservaTime");
        //return goReservaTime;
        //將可以預約的時間轉成訂單
        var orderList = (from res in goReservaTime
                         join loc in locations on res.LocationId equals loc.Id into LOC//這樣避免跑出重複的
                         select new EkiOrder()
                         {
                             LocationId = res.LocationId,
                             AddressId = LOC.First().AddressId,
                             MemberId = res.MemberId,
                             ReservaTime = res,
                             SerialNumber = this.generateOrderTimeSerNum(),
                             LocPrice = LOC.First().ReservaConfig.Price,
                             Cost = OrderCalculater.calOrderCost(res, LOC.First().ReservaConfig),
                             Unit = LOC.First().ReservaConfig.Unit,
                             Status = (int)OrderStatus.Reserved,
                             CarNum = res.CarNum,
                             Ip = WebUtil.GetUserIP(),
                             beEnable = true
                         }).toSafeList();



        return orderList;
    }

    public class OrderCalculater
    {
        public static OrderCalculater init(EkiOrder o, CheckOutRequest request)
        {
            var oldCheckOut = (from c in EkiSql.ppyp.table<EkiCheckOut>()
                          where c.MemberId == o.MemberId
                          select c);
            var now = request.date.toDateTime();
            var input = new OrderInput
            {
                order = o,
                checkOut = request.convertToDbModel().Also(c=>c.MemberId = o.MemberId )
            };
            //檢查結帳時間不得小於預約時間
            if(input.checkOut.Date<o.ReservaTime.StartTime)
                throw new ArgumentException();

            request.action.notNullOrEmpty(act =>
            {
                input.action = (from a in EkiSql.ppyp.table<EkiAction>()
                              where a.Code.Equals(act, StringComparison.OrdinalIgnoreCase)
                              where a.beEnable
                              where a.TimeLimit ? a.StartTime <= now && now <= a.EndTime : true
                              select a).FirstOrDefault();
                
                if(input.action==null)
                    throw new ArgumentException();

                if ((from c in oldCheckOut
                     where c.ActionId == input.action.Id
                     select c).Count() > 0)
                    throw new ArgumentException();

                input.checkOut.ActionId = input.action.Id;
            });

            request.discount.notNullOrEmpty(dis =>
            {
                input.discount = (from d in EkiSql.ppyp.table<MemberDiscount>()
                                where d.MemberId == o.MemberId
                                where d.Code.Equals(dis,StringComparison.Ordinal)
                                where d.beEnable
                                where d.IsRange ? now <= d.EndTime : true
                                select d).FirstOrDefault();
                if (input.discount == null)
                    throw new ArgumentException();

                if ((from c in oldCheckOut
                     where c.DiscountId == input.discount.Id
                     select c).Count() > 0)
                    throw new ArgumentException();

                input.checkOut.DiscountId = input.discount.Id;
            });
            return new OrderCalculater(input);
        }
        private OrderInput input;
        OrderCalculater(OrderInput i)
        {
            input = i;
        }

        public CalResult calCheckOut(EkiPostImg img=null)
        {
            var checkOutTime = input.checkOut.Date;
            
            var checkOutResult = calUserCheckoutOrderCost(input.order, checkOutTime);

            var cost = checkOutResult.cost.toDouble();
            var claimant = checkOutResult.claimant.toDouble();
            

            var result = new CalResult();
            if (input.action != null || input.discount != null)
            {
                //input.checkOut.ActionId = input.action == null ? 0 : input.action.Id;
                //input.checkOut.DiscountId = input.discount == null ? 0 : input.discount.Id;

                var actionFix = input.action.Let(a =>
                {
                    if (a != null)
                        switch (a.actionType)
                        {
                            case ActionType.Discount:
                                var discount = a.getTypeDetail<EkiAction.Discount>();
                                if (discount.Number != 0)
                                    return discount.Number;
                                else
                                    return cost - cost * discount.Ratio;
                        }
                    return 0d;
                });
                var discountFix = input.discount.Let(d =>
                {
                    if (d != null)
                        return d.Amt;
                    return 0d;
                });

                input.checkOut.CostFix = cost >= (actionFix + discountFix) ? -(actionFix + discountFix).toCurrency(input.order) : -cost.toCurrency(input.order);
                cost = (cost + input.checkOut.CostFix);
            }
            var totalCost = cost + claimant;

            input.order.Cost = totalCost.toDecimal();
            input.order.Ip = WebUtil.GetUserIP();

            input.checkOut.OrderId = input.order.Id;

            if (img != null)
                input.checkOut.Img = img?.fullName;
            //這要跟Url 相配合
            input.order.StatusEnum = totalCost > 0 ? OrderStatus.BeSettle : OrderStatus.CheckOut;
            input.checkOut.Url = totalCost > 0 ? input.order.getPayUrl() : "";
            input.checkOut.Claimant = claimant;

            result.order = input.order;
            result.checkOut = input.checkOut;
            result.discount = input.discount;

            return result;
        }
        //這邊是專門計算車主結帳
        private CheckoutResult calUserCheckoutOrderCost(EkiOrder order, DateTime checkOutTime)
        {
            var reserva = order.ReservaTime;
            var startTime = reserva.getStartTime();
            var endTime = reserva.getEndTime();

            if (checkOutTime < startTime)
                throw new ArgumentException();

            var standarCheckout = checkOutTime.standarCheckOutTime(reserva);

            //reserva.setEndTime(standarCheckout);
            //假如是正常預約時間內checkout就正常費率
            if (standarCheckout <= endTime)
                return new CheckoutResult(calOrderCost(standarCheckout - startTime, order).toCurrency(order), 0m);
                //return calOrderCost(standarCheckout - startTime, order).toCurrency(order);

            var normalCost = calOrderCost(endTime - startTime, order);
            var additionCost = calOrderCost(standarCheckout - endTime, order);
            //罰金部分
            var claimantCost = (additionCost * ApiConfig.OrderClaimantRate).toCurrency(order);

            return new CheckoutResult(normalCost.toCurrency(order), claimantCost);
            //return (normalCost + claimantCost).toCurrency(order);
        }
        //單純依照車位設定的計費價格去計算停車時間所需的總價格
        public static decimal calOrderCost(ReservedTime reserva, IPriceSet<decimal> config)
        {
            //var dateStr = TimeUtil.ToDateTime(Convert.ToInt64(reserva.Stamp)).ToString(ApiConfig.DateFormat);
            var startTime = reserva.getStartTime();
            var endTime = reserva.getEndTime();
            return calOrderCost(endTime - startTime, config).toCurrency(config);
        }
        //單純依照車位設定的計費價格去計算停車時間所需的總價格
        public static decimal calOrderCost(TimeSpan span, IPriceSet<decimal> config)
        {
            var minuteSpan = span.TotalMinutes;
            //var gap = config.PriceMethod.Min;
            var priceCount = minuteSpan / config.methodSet().Min;
            //var priceCount = minuteSpan % gap < ApiConfig.CountMinGap ? minuteSpan / gap : (minuteSpan / gap) + 1;
            var cost = config.price() * priceCount.toDecimal();
            return cost;
            //return cost.toCurrency(config);
        }

        private class CheckoutResult
        {
            public decimal cost;
            public decimal claimant;//車主違規賠償金
            public CheckoutResult(decimal cost,decimal claimant)
            {
                this.cost = cost;
                this.claimant = claimant;
            }
            public double total() => (cost + claimant).toDouble();
        }

        public class CalResult
        {
            public EkiOrder order;
            public EkiCheckOut checkOut;
            //public CheckOutConsume consume;
            public MemberDiscount discount = null;
        }
    }
    private class OrderInput
    {
        public EkiOrder order = null;
        public EkiCheckOut checkOut = null;
        public EkiAction action = null;
        public MemberDiscount discount = null;
    }
 

    public EkiCheckOut OrderCheckOut(EkiPostImg postImg, CheckOutRequest request)
    {
        var order = (from o in GetTable<EkiOrder>()
                     where o.beEnable
                     where o.SerialNumber == request.number
                     where (o.StatusEnum == OrderStatus.Reserved|| o.StatusEnum == OrderStatus.InUsing)
                     select o).FirstOrDefault();

        //this.saveLog("order->" + order,this);

        order.isNull(() => { throw new ArgumentNullException(); });

        var calculater = OrderCalculater.init(order, request);

        var result = calculater.calCheckOut(postImg);

        //var startTime = TimeUtil.CombineStampToDateTime(order.ReservaTime.Stamp, order.ReservaTime.StartTime);

        //var checkOutTime = request.date.toDateTime();
        //order.Cost = calOrderCost(order, checkOutTime);
        //order.StatusEnum = OrderStatus.BeSettle;
        //order.Ip = WebUtil.GetUserIP();

        //order.ReservaTime.EndTime = checkOutTime.ToString("yyyy-MM-dd hh:mm:ss");

        //var checkOut = request.convertToDbModel();
        //checkOut.OrderId = order.Id;
        //checkOut.Img = postImg.fullName;
        //checkOut.Url = order.getPayUrl();

        //這邊以後要加入車牌辨識api 
        //目前圖片可以不必要
        if(postImg!=null)
            postImg.saveOrderBitmapWith(member);

        result.checkOut.Insert();
        result.order.Update();

        //插入對應的EKI 付款序號
        //OrderPayRecord.creat(order.Id).Also(record =>
        //{
        //    record.Insert();
        //});

        //紀錄發票的訊息
        if (request.invoice != null)
        {
            request.invoice.convertData().Also(i =>
            {
                i.OrderId = result.order.Id;
                i.MemberId = result.order.MemberId;
                i.SerNO = result.order.SerialNumber;
                /*
                 當CarrierType為ezPay電子發票載具時，此參數請提供可識別買受人之代號(例：e-mail、手機號碼、會員編號…等)
                 */
                if (i.type == InvoiceType.ezPay)
                    i.CarrierNum = member.PhoneNum;

            }).Insert();
        }
        else
        {
            new EkiInvoice
            {
                OrderId = result.order.Id,
                MemberId = result.order.MemberId,
                SerNO = result.order.SerialNumber
            }.Insert();
        }        
        

        //result.consume.notNull(con =>
        //{
        //    con.CheckOutId = checkoutId;
        //    con.Insert();
        //});
        result.discount.notNull(dis =>
        {            
            dis.beEnable = false;
            dis.Update();
        });

        //postImg.saveTo($"~{string.Format(DirPath.Order, member.UniqueID)}");   

        var orderNormalCost = OrderCalculater.calOrderCost(result.checkOut.Date.standarCheckOutTime(result.order.ReservaTime) - result.order.ReservaTime.getStartTime(), result.order).toCurrency(result.order);

        var broadcastMsg = new CheckOutContent()
        {
            OrderSerNum = result.order.SerialNumber,
            Cost = orderNormalCost,
            Unit = result.order.Unit,
            HandlingFee = result.order.HandlingFee,
            StartTime = result.order.ReservaTime.StartTime.toString(),
            EndTime = result.checkOut.Date.toString(),
            Img = result.checkOut.mapImgUrlWith(DirPath.Order, member),
            //Img = $"{string.Format(DirPath.Order, member.UniqueID)}/{checkOut.Img}".toLinkUrl(),
            CarNum = result.order.CarNum
        };

        //var location = (from loc in GetTable<Location>()
        //                where loc.Id == order.LocationId
        //                select loc).First();

        var landMember = (from m in GetTable<Member>()
                          join loc in GetTable<Location>() on order.LocationId equals loc.Id
                          where m.Id == loc.MemberId
                          select m).FirstOrDefault();

        //使用socket通知
        landMember?.sendPushMsg(broadcastMsg);

        //假如該地點底下有CP 停止充電
        new StopChargeProcess(order).runAsync();

        return result.checkOut;
    }

    //因為預約最多可以到2個月後 所以最多找出2個月內的訂單來檢查能不能預約
    private bool IsValidReservaDay(EkiOrder order,DateTime time)
    {
        var rStart = order.ReservaTime.getStartTime();
        //-1是稍微修正時差刻意寬一點
        if (rStart.Date < time.AddDays(-1).Date)
            return false;

        var day = (rStart - time).TotalDays;

        return day <= ApiConfig.MaxReservaDay;
    }

    public bool setOrderCancelImg()
    {
        if (!this.formFileContain(RequestFlag.Body.Img) || !this.formDataContain(RequestFlag.Body.Info))
            throw new ArgumentNullException();

        var postImg = this.getPostImg(RequestFlag.Body.Img);
        var request = this.getPostObj<OrderCancelImgRequest>(RequestFlag.Body.Info);

        var cancel = (from o in GetTable<EkiOrder>()
                      where o.SerialNumber == request.serNum
                      where o.StatusEnum==OrderStatus.Cancel
                      join c in GetTable<OrderCancel>() on o.Id equals c.OrderId
                      select c).FirstOrDefault();
        if(cancel==null)
            throw new ArgumentNullException();
        if (!cancel.Img.isNullOrEmpty())
            throw new ArgumentException();

        try
        {
            cancel.Img = postImg.imgName();
            cancel.Update();
            postImg.saveOrderBitmapWith(member);

            return true;
        }
        catch(Exception e)
        {

        }
        return false;   
    }

    //回傳剩餘次數
    public object CancelOrder(SearchRequest request)
    {
        var cancelText = request.text.cleanXss();
        var numList = request.serNum;
        var now = request.time.isNullOrEmpty() ? DateTime.Now : request.time.toDateTime();

         var table = GetTable<EkiOrder>();
        //計算出今天內已經取消的單
        var delCount = (from order in table
                        join cancel in GetTable<OrderCancel>() on order.Id equals cancel.OrderId into Cancel 
                        where order.MemberId == member.Id
                        where order.beEnable
                        where Cancel.Count() < 1 ? false : (now - Cancel.FirstOrDefault().Time).TotalDays < 1
                        where order.StatusEnum == OrderStatus.Cancel
                        select order).Count();

        //找出想要取消的訂單
        var orders = (from num in numList
                      join order in table on num equals order.SerialNumber
                      where order.beEnable
                      where order.MemberId == member.Id
                      select order);
        //找出能夠取消的單
        var delOrder = (from order in orders
                        where order.StatusEnum == OrderStatus.Reserved
                        where order.isCancelable(now)//目前需要判斷是預約開始時間之前
                        select order).toSafeList();
        if (delOrder.Count() < 1)
            throw new OutOfDateTimeException();

        using (var checkProcess = new CheckProcess())
        {
            checkProcess
                .add(new OrderCancelCheck(delCount,delOrder))
                //.add(new OrderSameTimeCheck(userOrder, reservaList))
                .run();
        }

        // var cancelList = new List<OrderCancel>();
        var locs = EkiSql.ppyp.table<Location>();
        var members = EkiSql.ppyp.table<Member>();

        var list = new DbObjList<EkiOrder>(delOrder);
        list.UpdateByObj(order =>
        {
            new OrderCancel()
            {
                OrderId = order.Id,
                Paid = false,
                Time = now,
                Lat=request.lat,
                Lng=request.lng,
                Text=cancelText
            }.Insert();

            var spanMin = order.spanStartMin(now);
            if (spanMin < ApiConfig.OrderCancelMin)
            {
                //現在不用扣錢了2020/11/25
                order.toCancel(0);
                //取消時間在30分鐘以內 以30分鐘計算錢
                //order.Cost = order.getHalfHourPrice();
                //cancel.Cost = order.getHalfHourPrice().toDouble();

                //if (!request.invoice.isNullOrEmpty())
                //    request.invoice.convertData().Also(i =>
                //    {
                //        i.OrderId = order.Id;
                //        i.MemberId = order.MemberId;
                //        i.SerNO = order.SerialNumber;
                //    /*
                //     當CarrierType為ezPay電子發票載具時，此參數請提供可識別買受人之代號(例：e-mail、手機號碼、會員編號…等)
                //     */
                //        if (i.type == InvoiceType.ezPay)
                //            i.CarrierNum = member.PhoneNum;
                //    }).Insert();
            }
            else
            {
                //order.Cost = 0;
                order.toCancel();
            }

            
            //取消的訂單要不要審查
            if (request.isVerify)
            {
                new Verify
                {
                    typeEnum=VerifyType.Order,
                    statusEnum=VerifyStatus.Processing,
                    ItemId=order.Id
                }.Insert();
            }

            //發送要訂單刪除的FCM
            (from l in locs
             where l.Id == order.LocationId
             select l).FirstOrDefault().notNull(loc =>
             {
                 (from m in members
                  where m.Id == loc.MemberId
                  select m).FirstOrDefault().notNull(locMember =>
                  {
                      locMember.sendCancelOrderMsg(order, loc);
                  });
             });
        });

        var result = (from num in numList 
                      join order in GetTable<EkiOrder>() on num equals order.SerialNumber
                      join cancel in GetTable<OrderCancel>() on order.Id equals cancel.OrderId 
                      select new
                      {                       
                          //DelCount=delCount,
                          SerNum = order.SerialNumber,
                          //刪除成功OrderCancel裡面會有資料
                          Success = (from del in delOrder
                                     where del.Id == cancel.OrderId
                                     select del).Count() > 0 ? true : false,
                          Checkout=cancel.Cost>0?true:false,
                          Order=order.convertToResponse().Also(res=> { res.ReservaTime.setIsUser(true); })
                      }).toSafeList();

        return new
        {
            Remain = ApiConfig.OrderCancelLimit - delCount - delOrder.Count,
            Result = result
        };
    }

        
    /// <summary>
    /// 舊版本的檢查能否預約的函式
    /// 只能檢查同一個開放時間的預約時間是否合法
    /// 假如有跨日或者連者的開放時間無法處理
    /// 且消耗較大 目前棄用 請使用V2版
    /// </summary>
    /// <param name="location"></param>
    /// <param name="locTime"></param>
    /// <param name="userTime"></param>
    /// <returns></returns>
    private bool validReservaTime(Location location, 
        IEnumerable<ReservedTime> locTime,
        ReservedTime userTime)
    {
        //地主沒開就拒絕
        if (!location.ReservaConfig.beEnable)
            return false;

        var userStartTime = userTime.getStartTime();
        var userEndTime = userTime.getEndTime();

        #region 先檢查地主設定的開放時間
        var openSet = location.ReservaConfig.OpenSet;

        //取出有設周循環的 根開放日期一樣的 地主設定的開放時間
        var openList = (from set in openSet
                         where set.Week > (int)WeekEnum.NONE ? true :
                         userStartTime.Date == set.getStartTime().Date
                         select set);

        //var userStartTime = TimeUtil.CombineStampToDateTime(Convert.ToInt64(userTime.Stamp), userTime.StartTime);

        //檢查星期跟開放時間時間是否是有一樣
        var weekOpenSet = (from set in openList
                            where (int)set.weekEnum == (int)userStartTime.DayOfWeek
                            select set).toSafeList();
        //檢查特定日子的開放時間
        var dayOpenSet = (from set in openList
                          where set.weekEnum == WeekEnum.NONE
                          where set.getStartTime().Date == userTime.getStartTime().Date
                          select set).toSafeList();
        //假如該日子都沒有開啟
        if (weekOpenSet.Count < 1 && dayOpenSet.Count < 1)
            return false;

        var openCheckList = new List<OpenCheck>();
        //這個星期數有OpenTime(周循環)
        if (weekOpenSet.Count > 0)
        {
            //檢查裡面的時段
            openCheckList.Add(new OpenCheck(weekOpenSet));
        }

        if(dayOpenSet.Count>0)
        {
            openCheckList.Add(new OpenCheck(dayOpenSet));
        }
        if (!openCheckList.Any(c => c.IsOpen(userTime)))
            return false;
        #endregion

        //if (dbTime.Count() < 1)//查無該地點的已預約時間
        //    return true;

        //再檢查該位置所有的預約時間
        //選擇日期一樣的已預約時間
        //或者以預約時間的start<

        var locReservaTime = (from time in locTime                             
                              where time.getStartTime().Date >= userTime.getStartTime().Date
                              where !(time.getEndTime() < userStartTime)                             
                              where !(time.getStartTime() > userEndTime)                              
                              select time).toSafeList();

        if (locReservaTime.Count() < 1)//該日期無人預約
            return true;
        //locReservaTime.saveLog("check reserva final ok");

        var check = new ForbiddenCheck(locReservaTime);
        if (check.IsForbidden(userTime))
            return false;

        return true;
    }


    /// <summary>
    /// 新版本檢查預約時間是否能夠成立
    /// 簡化確定的流程 
    /// 目前只檢查開始,結束 是否都在該地點的開放時間之內就好
    /// 以提升處理效率 其他判斷交由前端執行
    /// </summary>
    /// <param name="location">要預約的地點</param>
    /// <param name="userTime">要預約的時間</param>
    /// <param name="locReservaCheck">該地點底下的預約單</param>
    /// <returns></returns>
    private bool validReservaTime_v2(Location location,
                                                           ReservedTime userTime,
                                                           IEnumerable<ReservedTime> locReservaCheck=null)
    {
        var start = userTime.getStartTime();
        var end = userTime.getEndTime();

        if (end < start)
            return false;

        //Log.print($"user end->{end.toString()}");

        //檢查已經預約的時間是否有卡到 將預約的時間
        if (locReservaCheck != null)
        {
            //確認已預約時間 是否有重疊到將預約時間
            if (locReservaCheck.Any(r =>
            {
                //已預約時間 皆小於將預約的開始時間
                var isStartLargeReserva = r.getStartTime() <= start && r.getEndTime() <= start;

                //已預約時間 皆大於將預約的結束時間
                var isEndSmallReserva = r.getStartTime() >= end && r.getEndTime() >= end;

                //表示將預約時間沒有重疊到已預約時間
                if (isStartLargeReserva || isEndSmallReserva)
                    return false;


                ////確認預約時間是否有重疊
                //if (r.between(start) || r.between(end))
                //    return true;
                ////確認預約時間是否有被中斷
                ////已預約時間被包含在 將預約時間之內
                //if (r.getStartTime() >= start && end >= r.getEndTime())
                //    return true;
                ////將預約時間被包含在已預約時間之內
                //if (start >= r.getStartTime() && r.getEndTime() >= end)
                //    return true;
                //return false;
                
                //假如時間有重疊到 表示該時段不可預約
                return true;
            }))
                return false;

        }


        var locOpenTime = (from o in location.ReservaConfig.OpenSet
                           where o.getStartTime(start).Date == start.Date || //開始時間比較沒跨日問題
                           //以下判斷結束時間可以避免24:00:00的問題
                           o.getStartTime(start).Date==end.Date||
                           o.getEndTime(end).Date == end.Date
                           select o);



        //var testOpen = location.ReservaConfig.OpenSet.FirstOrDefault(o => o.Id == 7573);
        //Log.print($"locId->{location.Id} locOpenTime->{locOpenTime.toJsonString()}");
        //Log.print($"open->{testOpen.toJsonString()} start->{testOpen.getStartTime().toString()} end->{testOpen.getEndTime().toString()}");

        var checkStart = locOpenTime.Any(open => open.between(start));
        var checkEnd = locOpenTime.Any(open => open.between(end));
        return checkStart && checkEnd;

        //var result = checkStart && checkEnd;
        //Log.print($"validReservaTime_v2 result->{result}");

        //return result;
    }


    public static OrderPayInfo getPayInfo(EkiOrder order)
    {
        var info = new OrderPayInfo();

        switch (order.StatusEnum)
        {
            case OrderStatus.PayError:
            case OrderStatus.BeSettle:
                var checkOut = order.CheckOut;
                //checkOut.saveLog("checkout Data");
                if (checkOut.isNullOrEmpty())
                    throw new ArgumentNullException();

                info.amt = order.Cost.toDouble();
                info.desc = OrderPayInfo.itemDesc(checkOut, order.Location);
                info.newebStamp= NewebPayUtil.ConvertToStamp(checkOut.Date).ToString();
                break;
            case OrderStatus.Cancel:
                var cancel = new OrderCancel().Also(c => c.CreatById(order.Id));

                info.amt = cancel.Cost;
                info.desc = "取消費用";
                info.newebStamp= NewebPayUtil.ConvertToStamp(cancel.Time).ToString();
                break;
            default://其他狀態不接受
                throw new ArgumentNullException();
        }

        return info;
    }

    public class OrderPayInfo
    {
        public double amt { get; set; }
        public string desc { get; set; }
        public string newebStamp { get; set; }

        public static string itemDesc(EkiCheckOut checkOut, Location location)
        {
            return $"車位編號：{location.SerNum}  結帳時間：{checkOut.Date.ToString("yyyy-MM-dd hh:mm:ss")}";
        }
    }
}