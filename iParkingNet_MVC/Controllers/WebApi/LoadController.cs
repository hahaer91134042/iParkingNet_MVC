using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

[RoutePrefix("api/Load")]
public class LoadController : BaseApiController
{
    #region LatLng
    [HttpPost]
    [Route("LatLng")]
    [EkiSecretFilter]
    public object LatLng(AddressRequest request)
    {
        try
        {
            if (request.isEmpty())
                throw new ArgumentNullException();
            //request.saveLog("Load/LatLng request");
            var latlng = request.convertToLatLng();

            return new LoadResponse(true)
            {
                info = latlng
            };
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch(Exception e)
        {

        }
        return ResponseError();
    }
    #endregion

    #region Notify
    [HttpPost]
    [Route("Notify")]
    [EkiSecretFilter]
    public object Notify()
    {
        try
        {
            var now = DateTime.Now;
            var serverNotiList = (from n in GetTable<ServerNotify>()
                                  where n.StartTime<=now
                                  where n.EndTime.isNullOrEmpty()?true:now<=n.EndTime
                                  select n).toSafeList();

            var action = (from a in GetTable<EkiAction>()
                          where a.beEnable
                          where !a.TimeLimit ? true : a.StartTime <= now && now <= a.EndTime
                          select a).toSafeList();
            var list = new List<object>();
            serverNotiList.ForEach(noti =>
            {
                list.Add(noti.convertToResponse());
            });
            action.ForEach(a =>
            {
                list.Add(a.getNotifyResponse());
            });

            return new LoadResponse(true)
            {
                info =list
            };

        }catch(Exception e)
        {
            saveUnknowError(e,"Load/Notify");
        }
        return ResponseError();
    }

    #endregion

    #region Action
    [HttpPost]
    [Route("Action")]
    [JwtAuthActionFilter]
    public object Action(SearchRequest request)
    {
        try
        {
            if (request.isSerNumEmpty())
                throw new InputFormatException();

            var member = getAuthObj().getMember();
            var now = DateTime.Now;

            //var actions = (from serial in request.serNum
            //               join a in GetTable<EkiAction>() on serial.ToUpper() equals a.Code.ToUpper()
            //               where a.beEnable
            //               where a.TimeLimit ? a.StartTime.toDateTime() <= now && now <= a.EndTime.toDateTime() : true
            //               select a);
            //var checkOut = (from c in GetTable<EkiCheckOut>()
            //                where member.Id == c.MemberId
            //                select c);
            
            var result = (from serial in request.serNum
                           join a in GetTable<EkiAction>() on serial.ToUpper() equals a.Code.ToUpper() into Action
                           join c in GetTable<EkiCheckOut>() on member.Id equals c.MemberId into CheckOut
                           select new
                           {
                               Serial = serial,
                               Action = (from a in Action
                                         where !CheckOut.Any(o => o.ActionId == a.Id)
                                         where a.beEnable
                                         where a.TimeLimit ? a.StartTime <= now && now <= a.EndTime : true
                                         select a).FirstOrDefault()?.convertToResponse()
                           }).toSafeList();

            //var result=()


            //var actionObj = (from a in GetTable<EkiAction>()
            //              where a.beEnable
            //              where a.TimeLimit ? a.StartTime.toDateTime() <= now && now <= a.EndTime.toDateTime() : true
            //              where a.Code.Equals(code,StringComparison.OrdinalIgnoreCase)
            //              select a).FirstOrDefault();
            if (result != null)
            {
                return new LoadResponse(true)
                {
                    info = result
                };
            }
            return ResponseError(EkiErrorCode.E003);
        }
        catch (Exception)
        {
        }
        return ResponseError();
    }
    #endregion

    #region ---Order---
    [HttpPost]
    [Route("Order")]
    [JwtAuthActionFilter]
    public object Order(SearchRequest request)
    {
        try
        {
            var auth = getAuthObj();
            

            var orderManager = new OrderManager(auth.user);
            var orderList = orderManager.LoadUserOrder(request == null ? null : request.serNum);


            return new LoadResponse(true)
            {
                info=orderList
            };
        }catch(Exception e)
        {
            saveUnknowError(e,request.toJsonString());
        }
        return ResponseError();
    }
    #endregion

    #region ---ReservaStatus---
    [HttpPost]
    [Route("ReservaStatus")]
    public object ReservaStatus(SearchRequest request)//丟入location Id
    {
        try
        {            

            if (request.isIdEmpty() && request.isSerNumEmpty())
                throw new ArgumentNullException();

            var loadManager = new LoadManager();


            var dataList = loadManager.getReservaTimeFrom(request);

            return new LoadResponse(true)
            {
                //info = new { id= request.isIdEmpty(),ser=request.isSerNumEmpty()}
                info = dataList
            };
        }
        //catch (ArgumentNullException)
        //{
        //    return ResponseError(EkiErrorCode.E001);
        //}
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E006);
        }
        catch (Exception e)
        {
            saveUnknowError(e,request.toJsonString());
            //return e;
        }
        return ResponseError();
    }
    #endregion

    #region ---Location---
    [HttpPost]
    [Route("Location")]
    public object Location(LoadLocationRequest request)
    {
        try
        {
            if (request.isEmpty())
                throw new ArgumentNullException();
            if (!request.isValid())
                throw new ArgumentException();

            //return new
            //{
            //    request
            //};
            //DateTime end;
            //DateTime start;

            //return new
            //{
            //    IsEnd= DateTime.TryParseExact(request.config.searchTime.end, ApiConfig.TimeFormat, null, System.Globalization.DateTimeStyles.None, out end),
            //    end,
            //    IsStart= DateTime.TryParseExact(request.config.searchTime.start, ApiConfig.TimeFormat, null, System.Globalization.DateTimeStyles.None, out start),
            //    start,
            //    Large=end>start
            //};


            var loadManager = new LoadManager();

            //var locInRange = request.address.isEmpty() ? loadManager.getLocationFrom(request.lat, request.lng, request.config,request.nowTime) : loadManager.getLocationFrom(request.address, request.config,request.nowTime);
            var result = loadManager.getLocationFrom(request);

            //var dataList = (from loc in locInRange.List
            //                select loc.convertToLoadLocResponse()).toSafeList();

            //var dataList = (from loc in locInRange.List
            //                select new
            //                {
            //                    loc.Id,
            //                    loc.Lat,
            //                    loc.Lng,
            //                    //loc.Distance,
            //                    //loc.Range,
            //                    //loc.IsOk,
            //                    loc.Img,
            //                    loc.Address,
            //                    loc.Info,                                
            //                    Config = RemoveObjAttr(loc.Config, "OpenSet", "beEnable"),
            //                    loc.Available,
            //                    loc.RatingCount
            //                }).toSafeList();

            return new LoadResponse(true)
            {
                info=result
            };

            //return new LoadResponse(true)
            //{
            //    info = new LocMultiPageResponse()
            //    {
            //        Page = locInRange.Page,
            //        Total = locInRange.Total,
            //        Lat=locInRange.Lat,
            //        Lng=locInRange.Lng,
            //        List = dataList,                    
            //    }
            //};
        }
        catch (GoogleApiException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e);
        }
        return ResponseError();
    }
    #endregion


    public class LoadResponse : ResponseInfoModel<object>
    {
        public LoadResponse(bool successful) : base(successful)
        {
        }
    }
}
