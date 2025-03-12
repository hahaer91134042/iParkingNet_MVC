using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

[RoutePrefix("api/v2/Manager")]
public class Manager_v2_Controller : BaseApiController
{
    #region CancelOpenSet
    [HttpPost]
    [Route("CancelOpenSet")]
    [JwtAuthManagerFilter]
    public object CancelOpenSet(EditOpenSetRequest request)
    {
        try
        {
            if (!request.isValid())
                throw new ArgumentException();

            var auth = getAuthObj();
            var locationManager = auth.creatLocationOwnerManager();
            var loc = locationManager.deleteOpenSet(request,2);
            if (loc != null)
            {
                return new ManagerResponse(true)
                {
                    info = loc
                };
            }
            else
            {
                return ResponseError(EkiErrorCode.E001);
            }
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
    #region AddOpenSet
    [HttpPost]
    [Route("AddOpenSet")]
    [JwtAuthManagerFilter]
    public object AddOpenSet(EditOpenSetRequest request)
    {
        try
        {
            if (!request.isValid())
                throw new ArgumentException();

            var auth = getAuthObj();
            var locationManager = auth.creatLocationOwnerManager();
            var loc = locationManager.addOpenSet(request);
            if (loc != null)
            {
                //現在不再輸出資料 只回傳是否成功
                return new ManagerResponse(true)
                {
                    //info = loc.convertToResponse_v2().convertToManagerLocation_v2()
                };
            }
            else
            {
                return ResponseError(EkiErrorCode.E001);
            }
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E003);
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (TimeOverlapException)
        {
            return ResponseError(EkiErrorCode.E021);
        }
        catch (Exception e)
        {
            saveUnknowError(e);
        }
        return ResponseError();
    }
    #endregion

    #region EditLocation
    [HttpPost]
    [Route("EditLocation")]
    [JwtAuthActionFilter]
    public object EditLocation(EditLocationRequest locationRequest)
    {
        try
        {
            var auth = getAuthObj();
            //locationRequest.saveLog("edit loc get ");

            var locationManager = auth.creatLocationOwnerManager();

            //if (!formDataContain(RequestFlag.Body.Info))
            //    throw new ArgumentNullException();

            //var locationRequest = getPostObj<EditLocationRequest>(RequestFlag.Body.Info);

            //locationRequest.saveLog("EditLocation request isValid->"+locationRequest.isValid(),this);

            if (!locationRequest.isValid_v2())
                throw new InputFormatException();


            var location = locationManager.updateLocation(locationRequest,2);
            //locationRequest.saveLog("EditLocation request contain img->" + formFileContain(RequestFlag.Body.Img), this);

            //if (formFileContain(RequestFlag.Body.Img))
            //{
            //    //location.deleteImgWith(locationManager.member);
            //    //deleteFile($"~{DirPath.Member}/{auth.user}/{location.Info.Img}");
            //    var img = getPostImg(RequestFlag.Body.Img);
            //    img.saveBitmapWith(locationManager.member);
            //    //img.saveTo($"~{DirPath.Member}/{auth.user}");
            //    location.Info.Img = img.fullName;
            //}

            if (location.Update())
                return ResponseOK();
        }
        catch (ArgumentNullException e)
        {
            //e.saveLog("edit loc v2 error Argu", "v2", locationRequest);
            //locationRequest.saveLog("edit loc v2 error Argu", e);
            return ResponseError(EkiErrorCode.E001);
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E017);
        }
        catch (InputFormatException e)
        {
            //e.saveLog("edit loc v2 error Input", "v2", locationRequest);
            //locationRequest.saveLog("edit loc v2 error Input", e);
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            //var locationRequest = getPostObj<EditLocationRequest>(RequestFlag.Body.Info);
            saveUnknowError(e, locationRequest.toJsonString());
            //return e;
        }
        return ResponseError();
    }
    #endregion
    #region AddLocation
    [HttpPost]
    [Route("AddLocation")]
    [JwtAuthActionFilter]
    public object AddLocation(AddLocationRequest locationRequest)
    {
        try
        {
            var auth = getAuthObj();

            var locationManager = auth.creatLocationOwnerManager();



            if (!locationRequest.isValid_v2())
                throw new InputFormatException();

            //var result = locationManager.convertToLocation_v2(locationRequest);


            //result.Id = result.Insert(true);

            //var response = result.convertToResponse_v2();

            var response = locationManager.addLocation_v2(locationRequest);

            return new ManagerResponse(true)
            {
                info = response.convertToManagerLocation_v2()
            };
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (OutOfNumberException)
        {
            return ResponseError(EkiErrorCode.E016);
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E017);
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (GoogleApiException e)
        {
            saveUnknowError(e, "v2/Manager/AddLocation/GoogleApiError");
            //return e;//debug use
        }
        catch (Exception e)
        {
            saveUnknowError(e, "v2/Manager/AddLocation");
            //return e;
        }
        return ResponseError();
    }
    #endregion
    #region GetLocation
    [HttpPost]
    [Route("GetLocation")]
    [JwtAuthActionFilter]
    public object GetLocation(SearchRequest request)
    {
        try
        {
            var auth = getAuthObj();

            var locationManager = auth.creatLocationOwnerManager();

            //locationManager.mapImg();


            return new ManagerResponse(true)
            {
                info = request.Let(r =>
                {
                    var now = r == null ? DateTime.Now : r.time.isNullOrEmpty() ? DateTime.Now : r.time.toDateTime();

                    var list = new List<object>();

                    var orders = EkiSql.ppyp.table<EkiOrder>();

                    locationManager.filterOpenByMonth(now)
                    .convertResponseList_v2<LocationResponseModel>()
                    .ForEach(loc =>
                    {
                        list.Add(loc.convertToManagerLocation_v2((from o in orders
                                                                  where o.LocationId == loc.Id
                                                                  where o.beEnable
                                                                  where o.StatusEnum == OrderStatus.Reserved || o.StatusEnum == OrderStatus.InUsing
                                                                  where o.ReservaTime.getStartTime() >= now
                                                                  select o).Count() < 1));
                        //list.Add(new
                        //{
                        //    loc.Id,
                        //    loc.Lat,
                        //    loc.Lng,
                        //    loc.Img,
                        //    loc.Address,
                        //    Info=loc.Info.removeObjAttr("Current","Charge"),
                        //    loc.Config,
                        //    loc.RatingCount,
                        //    loc.Socket,
                        //    //這邊計算該地點能不能進行刪除
                        //    Deleteable = (from o in orders
                        //                  where o.LocationId == loc.Id
                        //                  where o.beEnable
                        //                  where o.StatusEnum == OrderStatus.Reserved || o.StatusEnum == OrderStatus.InUsing
                        //                  where o.ReservaTime.getStartTime() >= now
                        //                  select o).Count() < 1
                        //});
                    });

                    return list;
                    //return locationManager.filterOpenByMonth(now).convertResponseList<LocationResponseModel>();

                    //if (r == null)
                    //    return locationManager.locationList.convertResponseList<LocationResponseModel>();
                    //else
                    //    return request.time.isNullOrEmpty() ?
                    //    locationManager.locationList.convertResponseList<LocationResponseModel>() :
                    //    locationManager.filterOpenByMonth(request.time.toDateTime()).convertResponseList<LocationResponseModel>();
                })
            };
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E017);
        }
        catch (Exception e)
        {
            saveUnknowError(e);
        }
        return ResponseError();
    }
    #endregion
    public class ManagerResponse : ResponseInfoModel<object>
    {
        public ManagerResponse(bool successful) : base(successful)
        {
        }
    }
}
