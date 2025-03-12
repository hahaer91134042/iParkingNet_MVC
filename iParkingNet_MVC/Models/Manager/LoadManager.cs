using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LoadManager 的摘要描述
/// </summary>
public class LoadManager : BaseManager
{

    public List<object> getReservaTimeFrom(SearchRequest request)
    {
        var id = request.getUserUniqueId();
        var member = (from m in GetTable<Member>()
                      where m.UniqueID.equal(id)
                      select m).FirstOrDefault();
        var now = request.time.isNullOrEmpty() ? DateTime.Now : request.time.toDateTime();

        if (!request.isIdEmpty())
            return getReservaTimeFrom(request.id, now, member);
        else
            return getReservaTimeFrom(request.serNum, now, member);
    }
    public List<object> getReservaTimeFrom(List<string> serNum, DateTime now, Member user = null)
    {
        if (serNum.Count > ApiConfig.DataPageNum)
            throw new InputFormatException();
        //防止非法的serial number
        var volidSerNum = (from ser in serNum
                           where isValidSerialNum(ser)
                           select ser).toSafeList();

        return getLocReservaTime((from loc in GetTable<Location>()
                                  where volidSerNum.Contains(loc.SerNum) && loc.ReservaConfig.beEnable
                                  select loc).toSafeList(), now, user);
    }
    public List<object> getReservaTimeFrom(List<int> ids, DateTime now, Member user = null)
    {
        if (ids.Count > ApiConfig.DataPageNum)
            throw new InputFormatException();


        return getLocReservaTime((from loc in GetTable<Location>()
                                  where ids.Contains(loc.Id) && loc.ReservaConfig.beEnable
                                  select loc).toSafeList(), now, user);
    }

    public List<object> getLocReservaTime(List<Location> locList, DateTime now, Member user = null)
    {
        var checkUserTime = !user.isNullOrEmpty();



        var openTime = GetTable<OpenTime>();
        var reservaTimes = (from time in GetTable<ReservedTime>()
                            where IsValidReservaTime(time)
                            select time);
        var list = new List<object>();

        locList.ForEach(loc =>
        {
            list.Add(new
            {
                loc.Id,
                loc.Info.SerialNumber,
                OpenSet = (from time in openTime
                           where loc.ReservaConfigId == time.ParentId
                           where IsValidOpenTime(time, now)
                           select new OpenTimeResponseModel(time))?.toSafeList(),
                //ReservaTime = checkUserTime ? (from time in reservaTimes
                //                               where loc.Id == time.LocationId
                //                               where IsValidReservaTime(time)
                //                               join order in GetTable<EkiOrder>() on time.Id equals order.ReservedId
                //                               where order.beEnable
                //                               // time.convertToResponse().setIsUser(order.MemberId == user.Id)
                //                               select time.toResponse(order, order.MemberId == user.Id))?.toSafeList()

                //                             : (from time in reservaTimes
                //                                where loc.Id == time.LocationId && IsValidReservaTime(time)
                //                                select time.convertToResponse())?.toSafeList()


                ReservaTime = (from time in reservaTimes
                               where loc.Id == time.LocationId
                               //where IsValidReservaTime(time)
                               join order in GetTable<EkiOrder>() on time.Id equals order.ReservedId
                               where order.beEnable
                               where order.StatusEnum!=OrderStatus.Cancel&&order.StatusEnum!=OrderStatus.CancelByManager                           
                               // time.convertToResponse().setIsUser(order.MemberId == user.Id)
                               select time.toResponse(order, checkUserTime?order.MemberId == user.Id:false))?.toSafeList()

            });
        });

        return list;
    }

    //考慮到不同地區 有可能會導致判斷不准
    private bool IsValidReservaTime(ReservedTime time)
    {
        if (time.IsCancel)
            return false;

        if (time.Week > (int)WeekEnum.NONE)
            return true;
        var now = DateTime.Now.AddDays(-1).toStamp();//少一天才開始計算比較沒問題
        var end = time.getEndTime().toStamp();
        return end > now;
    }

    private bool IsValidOpenTime(OpenTime time, DateTime now)
    {
        if (time.Week > (int)WeekEnum.NONE)
            return true;
        //var now = DateTime.Now.toStamp();
        var end = time.getEndTime();
        return end > now;
    }

    public LocMultiPageResponse<object> getLocationFrom(LoadLocationRequest request, int ver = 1)
    {
        LatLng ori;
        if (request.address.isEmpty())
            ori = new LatLng(request.lat, request.lng);
        else
            ori = request.address.convertToLatLng();

        var config = request.config;
        if (config == null)
            config = new LoadConfigRequest
            {
                range = ApiConfig.MaxSearch.Range,
                unit = ApiConfig.MaxSearch.Unit.ToString()
            };

        return getLocationFrom(ori, config, request.nowTime, ver);
    }

    //public LocMultiPageResponse<object> getLocationFrom(double lat, double lng, LoadConfigRequest config = null,string  now="")
    //{
    //    var ori = new LatLng(lat, lng);
    //    if (config == null) 
    //        return getLocationFrom(ori,new LoadConfigRequest()
    //        { 
    //            range= ApiConfig.MaxSearch.Range,
    //            unit= ApiConfig.MaxSearch.Unit.ToString()
    //        },now);
    //    else 
    //        return getLocationFrom(ori, config,now);
    //}

    public LocMultiPageResponse<object> getLocationFrom(LatLng ori,
                                                                                                LoadConfigRequest config,
                                                                                                string now = "", int ver = 1)
    {
        var page = config == null ? 1 : config.page;
        List<ReservedTime> times = config.searchTime == null ? null : config.searchTime.convertToDbModel();

        var locData = (from loc in GetTable<Location>()
                       where loc.beEnable
                       where loc.ReservaConfig.beEnable
                       where ver == 1 ? IsValidCharge(loc.Info, config.charges) : IsValidCharge(loc, config.charges)
                       where config.range == -1 ? true : DistanceUtil.IsInRange(ori, new LatLng(loc.Lat, loc.Lng), config.range, config.unitEnum)
                       where IsValidSearchTime(loc, times)
                       orderby DistanceUtil.calDistance(ori, new LatLng(loc.Lat, loc.Lng),DistanceUnit.M) 
                       select loc);

        //有餘數+1
        var total = locData.Count() % ApiConfig.DataPageNum == 0 ? locData.Count() / ApiConfig.DataPageNum : locData.Count() / ApiConfig.DataPageNum + 1;
        //var total = locData.Count() ;
        var pageData = locData
            .Skip((page < 1 ? 0 : page - 1) * ApiConfig.DataPageNum)
            .Take(ApiConfig.DataPageNum)
            .toSafeList();


        pageData.ForEach(loc =>
        {
            if (!now.isNullOrEmpty())
                loc.setAvailable(now.toDateTime());
        });

        return new LocMultiPageResponse<object>()
        {         
            Page = page,
            Total = total,
            Lat = ori.Lat,
            Lng = ori.Lng
        }.Also(r =>
        {
            r.List = new List<object>();
            switch (ver)
            {
                case 2:
                    new DbObjList<Location>(pageData)
                    .convertResponseList_v2<LocationResponseModel>()
                    .ForEach(result =>
                    {
                        r.List.Add(result.convertToLoadLocResponse_v2());
                    });
                    break;
                default:
                    new DbObjList<Location>(pageData)
                    .convertResponseList<LocationResponseModel>()
                    .ForEach(result =>
                    {
                        r.List.Add(result.convertToLoadLocResponse());
                    });
                    break;
            }
        });
    }

    /**
     * edit time:2021/12/14 
     * 修改算法 增加運算速度
     * **/
    private bool IsValidSearchTime(Location loc, List<ReservedTime> userSearchTime)
    {
        if (userSearchTime == null)
            return true;
        if (userSearchTime.Count < 1)
            return true;


        //var userSearchTime = searchTime.convertToDbModel();
        //userSearchTime.saveLog("search time list");
        //var minSearchTime = userSearchTime.getMinTime();

        //這邊要改
        //var locOpen = (from o in loc.ReservaConfig.OpenSet
        //               where userSearchTime.Any(s=>s.getStartTime().Date== o.getStartTime().Date)
        //               select o).toSafeList();
        //var reservaList = (from reserva in GetTable<ReservedTime>()
        //                   where reserva.LocationId == loc.Id
        //                   where userSearchTime.Any(s => s.getStartTime().Date == reserva.getStartTime().Date)
        //                   select reserva);

        var openCheck = new OpenCheck((from o in loc.ReservaConfig.OpenSet
                                       where userSearchTime.Any(s => s.getStartTime().Date == o.getStartTime().Date)
                                       select o).toSafeList());
        var forbiCheck = new ForbiddenCheck((from reserva in GetTable<ReservedTime>()
                                             where reserva.LocationId == loc.Id
                                             where userSearchTime.Any(s => s.getStartTime().Date == reserva.getStartTime().Date)
                                             select reserva).toSafeList());

        //var isValid = false;

        //時間交集
        //return (from searchTime in userSearchTime
        //         where openCheck.IsOpen(searchTime)
        //         where !forbiCheck.IsForbidden(searchTime)
        //         select searchTime).Count()==userSearchTime.Count;
        //時間聯集
        return (from searchTime in userSearchTime
                where openCheck.IsOpen(searchTime)
                where !forbiCheck.IsForbidden(searchTime)
                select searchTime).Count() > 0;



        //userSearchTime.ForEach(userSearch =>
        //{
        //    //要先確認星期
        //    //var week = userSearch.getStartTime().DayOfWeek.toInt();

        //    var locOpenCheck = locOpen.getOpenCheck(userSearch.getStartTime());
        //    var locValid = locOpenCheck.IsOpen(userSearch);

        //    //var weekCheck = new ForbiddenCheck(userSearch.Date,(from f in locForbi
        //    //                                                     where f.Week==week
        //    //                                                     select f).toSafeList());

        //    //var weekValid = !weekCheck.IsForbidden(userSearch);

        //    //var dayCheck = new ForbiddenCheck((from f in locForbi
        //    //                                   where f.weekEnum==WeekEnum.NONE
        //    //                                   where userSearch.getStartTime().AddDays(-1)<=f.getStartTime()//看看有沒有跨日 同時去掉太久的拒絕日
        //    //                                   select f).toSafeList());
        //    //var dayValid = !dayCheck.IsForbidden(userSearch);

        //    var reservaCheck = new ForbiddenCheck((from r in reservaList
        //                                               //where r.getStartTime().Date==userSearch.getStartTime().Date
        //                                           where userSearch.getStartTime() <= r.getStartTime()
        //                                           select r).toSafeList());

        //    var reservaValid = !reservaCheck.IsForbidden(userSearch);

        //    //isValid = (weekValid || dayValid) && reservaValid;
        //    isValid = locValid && reservaValid;
        //    //isValid = reservaValid;
        //});
        //return isValid;
    }

    private bool IsValidCharge(LocationInfo info, List<int> charges)
    {
        if (charges == null)
            return true;
        if (charges.Count < 1)
            return true;
        return charges.Contains(info.Charge);
    }
    private bool IsValidCharge(Location loc, List<int> charges)
    {
        if (charges == null)
            return true;
        if (charges.Count < 1)
            return true;

        var sockets = (from s in GetTable<LocSocket>()
                       where s.LocationId == loc.Id
                       select s);
        foreach (var c in charges)
        {
            if (sockets.Any(s => s.Charge == c))
                return true;
        }
        return false;
    }

    //public LocMultiPageResponse<object> getLocationFrom(AddressRequest address, LoadConfigRequest config,string now="")
    //{
    //    try
    //    {
    //        var connect = WebConnect.CreatGet(GoogleApi.Parse(address).ApiUrl);
    //        var result = connect.Connect<GoogleMapResult>();
    //        //result.saveLog("google api result");

    //        if (!result.isSuccess())
    //            throw new GoogleApiException();

    //        return getLocationFrom(result.results[0].geometry.location.lat, 
    //            result.results[0].geometry.location.lng,
    //            config,now);
    //    }
    //    catch (Exception)
    //    {
    //        throw new GoogleApiException();
    //    }
    //}


}