using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

[RoutePrefix("api/Manager")]
public class ManagerController : BaseApiController
{
    #region LocMulctOrder
    [HttpPost]
    [Route("LocMulctOrder")]
    [JwtAuthManagerFilter]
    public object LocMulctOrder(SearchRequest request)
    {
        try
        {
            var locationManager = getAuthObj().creatLocationOwnerManager();

            return new ManagerResponse(true)
            {
                info=locationManager.getLocMulctOrder(request)
            };
        }
        catch (InputFormatException)
        {
            ResponseError(EkiErrorCode.E006);
        }
        catch (Exception e)
        {
            saveUnknowError(e, "LocMulctOrder");
        }
        return ResponseError();
    }
    #endregion

    #region LocIncome
    [HttpPost]
    [Route("LocIncome")]
    [JwtAuthManagerFilter]
    public object LocIncome(SearchRequest request)
    {
        try
        {
            var locationManager = getAuthObj().creatLocationOwnerManager();
            

            return new ManagerResponse(true)
            {
                info=locationManager.getLocIncome(request)
            };
        }
        catch (InputFormatException)
        {
            ResponseError(EkiErrorCode.E006);
        }
        catch(Exception e)
        {
            saveUnknowError(e, "LocIncome");
        }
        return ResponseError();
    }
    #endregion

    #region LvInfo
    [HttpPost]
    [Route("LvInfo")]
    [JwtAuthManagerFilter]
    public object LvInfo()
    {
        try
        {
            var locationManager = getAuthObj().creatLocationOwnerManager();
            var lvInfo = locationManager.getLvInfo();
            if (lvInfo != null)
            {
                return new ManagerResponse(true)
                {
                    info=lvInfo
                };
            }
            else
            {
                return ResponseError(EkiErrorCode.E003);
            }
        }
        //catch (PermissionException)
        //{
        //    return ResponseError(EkiErrorCode.E017);
        //}
        catch(Exception e)
        {

        }
        return ResponseError();
    }
    #endregion

    #region AddReferrer
    [HttpPost]
    [Route("AddReferrer")]
    [JwtAuthManagerFilter]
    public object AddReferrer(AddRequest request)
    {
        try
        {
            if (request.isCodeNullOrEmpty())
                throw new InputFormatException();

            var auth = getAuthObj();
            var locationManager = auth.creatLocationOwnerManager();
            var success = locationManager.addReferrer(request.code);
            if (success)
                return new ManagerResponse(true).removeObjAttr("info");
            else
                return new ManagerResponse(false).setErrorCode(EkiErrorCode.E005).removeObjAttr("info");
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E022);
        }
        catch(Exception e)
        {
            saveUnknowError(e,"AddReferrer");
        }
        return ResponseError();
    }

    #endregion

    #region GetBank
    [HttpPost]
    [Route("GetBank")]
    [JwtAuthManagerFilter]
    public object GetBank()
    {
        try
        {
            var auth = getAuthObj();
            var member = new Member().Also(m => m.CreatByUniqueId(auth.user));
            if (!member.beManager)
                throw new ArgumentException();
            return new ManagerResponse(true)
            {
                info=(from b in GetTable<BankInfo>()
                      where b.MemberId==member.Id
                      select b).FirstOrDefault()?.convertToResponse()
            };
        }
        catch (ArgumentException) 
        {
            return ResponseError(EkiErrorCode.E022);
        }
        catch (Exception e) 
        {
            //saveUnknowError(e,"GetBank");
        }
        return ResponseError();
    }
    #endregion

    #region EditBank
    [HttpPost]
    [Route("EditBank")]
    [JwtAuthManagerFilter]
    public object EditBank(BankRequest request)
    {
        try
        {
            if (!request.isValid())
                throw new ArgumentException();

            var auth = getAuthObj();
            var locationManager = auth.creatLocationOwnerManager();
            
            var success = locationManager.editBank(request);

            BankResponseModel response = null;
            if (success)
                response = (from b in GetTable<BankInfo>()
                         where b.MemberId == locationManager.member.Id
                         select b).FirstOrDefault()?.convertToResponse();
            return new ManagerResponse(success)
            {
                info=response
            };
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E017);
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

    #region OrderCancel
    [HttpPost]
    [Route("OrderCancel")]
    [JwtAuthManagerFilter]
    public object OrderCancel(SearchRequest request)
    {
        try
        {
            var auth = getAuthObj();
            var locationManager = auth.creatLocationOwnerManager();
            if (request.isSerNumEmpty() || !request.isTimeOrEmpty())
                throw new ArgumentException();

            var result = locationManager.cancelOrder(request);


            return new ManagerResponse(true)
            {
                info = result
            };
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E017);
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

    #region LocationOrder
    [HttpPost]
    [Route("LocationOrder")]
    [JwtAuthManagerFilter]
    public object LocationOrder(SearchRequest request)
    {
        try
        {
            var auth = getAuthObj();

            var locationManager = auth.creatLocationOwnerManager();



            if ((request.isIdEmpty() && request.isSerNumEmpty()) || (!request.isTimeOrEmpty() || !request.isTimeSpanOrEmpty()))
                throw new ArgumentException();

            var list = locationManager.creatLocationOrder(request);

            return new ManagerResponse(true)
            {
                info = list
            };
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E017);
        }
        catch (ArgumentException)
        {
            request.saveLog("error E001");
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e);
        }
        return ResponseError();
    }
    #endregion

    #region GetLocation
    [HttpPost]
    [Route("GetLocation")]
    [JwtAuthManagerFilter]
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

                    locationManager.filterOpenByMonth(now).convertResponseList<LocationResponseModel>().ForEach(loc =>
                    {
                        list.Add(loc.convertToManagerLocation((from o in orders
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
                        //    loc.Info,
                        //    loc.Config,
                        //    loc.RatingCount,
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

    #region DeleteLocation
    [HttpPost]
    [Route("DeleteLocation")]
    [JwtAuthManagerFilter]
    public object DeleteLocation(DeleteRequest request)
    {
        try
        {
            var auth = getAuthObj();

            if (request.isIdEmpty())
                throw new ArgumentNullException();

            var locationManager = auth.creatLocationOwnerManager();
            locationManager.deleteLocation(request.id);


            return ResponseOK();
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E017);
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e, request.toJsonString());
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
                return new ManagerResponse(true)
                {
                    info = loc.convertToResponse().convertToManagerLocation()
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
            var loc = locationManager.deleteOpenSet(request);
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
    #region EditOpenSet
    [HttpPost]
    [Route("EditOpenSet")]
    [JwtAuthManagerFilter]
    public object EditOpenSet(EditOpenSetRequest request)
    {
        try
        {
            if (!request.isValid())
                throw new ArgumentException();

            var auth = getAuthObj();
            var locationManager = auth.creatLocationOwnerManager();
            var loc = locationManager.updateOpenSet(request);
            if (loc!=null)
            {
                return new ManagerResponse(true)
                {
                    info = loc.convertToResponse()
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
    #region EditLocation
    [HttpPost]
    [Route("EditLocation")]
    [JwtAuthManagerFilter]
    public object EditLocation(EditLocationRequest locationRequest)
    {
        try
        {
            var auth = getAuthObj();

            var locationManager = auth.creatLocationOwnerManager();

            //if (!formDataContain(RequestFlag.Body.Info))
            //    throw new ArgumentNullException();

            //var locationRequest = getPostObj<EditLocationRequest>(RequestFlag.Body.Info);

            //locationRequest.saveLog("EditLocation request isValid->"+locationRequest.isValid(),this);

            if (!locationRequest.isValid())
                throw new InputFormatException();


            var location = locationManager.updateLocation(locationRequest);
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

            if(location.Update())
                return ResponseOK();
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E017);
        }
        catch (InputFormatException)
        {
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
    [JwtAuthManagerFilter]
    public object AddLocation(AddLocationRequest locationRequest)
    {
        try
        {
            var auth = getAuthObj();

            var locationManager = auth.creatLocationOwnerManager();

            //if (!formDataContain(RequestFlag.Body.Info))
            //    throw new ArgumentNullException();

            //var locationRequest = getPostObj<AddLocationRequest>(RequestFlag.Body.Info);

            if (!locationRequest.isValid())
                throw new InputFormatException();

            //var result = locationManager.convertToLocation(locationRequest);

            var response = locationManager.addLocation(locationRequest);

            //if (formFileContain(RequestFlag.Body.Img))
            //{
            //    var img = getPostImg(RequestFlag.Body.Img);
            //    img.saveBitmapWith(locationManager.member);
            //    //img.saveTo($"~{DirPath.Member}/{auth.user}");
            //    result.Info.Img = img.fullName;
            //}

            //result.Id = result.Insert(true);

            //var forbiTimeList = new DbObjList<ForbiTime>();
            //forbiTimeList.load(locationRequest.forbiSet);            
            //forbiTimeList.InsertToDb(new ActionCallBack<ForbiTime>(data =>
            //{
            //    data.ParentId = result.ReservaConfigId;
            //}));
            //var cmd = SqlCmd.InsertTo<ForbiTime>.ObjTable(forbiTimeList);

            //var response = locationManager.creatLocationResponse(result);
            //var response = result.convertToResponse();

            //response.Info.Img = getMemberImg(auth.user, response.Info.Img);

            return new ManagerResponse(true)
            {
                info = response.convertToManagerLocation()
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
            //locationRequest.saveLog("E001 error");
            return ResponseError(EkiErrorCode.E001);
        }
        catch (GoogleApiException e)
        {
            saveUnknowError(e, "Manager/AddLocation/GoogleApiError");
            //return e;//debug use
        }
        catch (Exception e)
        {
            saveUnknowError(e, "Manager/AddLocation");
            //return e;
        }
        return ResponseError();
    }
    #endregion

    #region SetLocationImg
    [HttpPost]
    [Route("SetLocationImg")]
    [JwtAuthManagerFilter]
    public object SetLocationImg()
    {
        try
        {
            var auth = getAuthObj();
            var locationManager = auth.creatLocationOwnerManager();

            var locImgList = locationManager.saveLocationImg();
           
            if (locImgList!=null)
            {
                return new ManagerResponse(true)
                {
                    info = (from l in locImgList
                            select new LocationResponseModel.LocImg
                            {
                                Sort = l.Sort,
                                Url = l.mapMemberImgUrl(locationManager.member)
                            }).toSafeList()
                };
            }
            else
            {
                return new ManagerResponse(false)
                    .setErrorCode(EkiErrorCode.E023);
            }
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E017);
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e, "Manager/SetLocationImg");
        }
        return ResponseError();
    }

    #endregion

    #region DeleteLocationImg
    [HttpPost]
    [Route("DeleteLocationImg")]
    [JwtAuthManagerFilter]
    public object DeleteLocationImg(LocationImgRequest imgRequest)
    {
        try
        {
            if (!imgRequest.isValid())
                throw new InputFormatException();

            var auth = getAuthObj();
            var locationManager = auth.creatLocationOwnerManager();

            var imgList = locationManager.deleteLocationImg(imgRequest);

            if (imgList != null)
            {
                return new ManagerResponse(true)
                {
                    info = (from l in imgList
                            select new LocationResponseModel.LocImg
                            {
                                Sort = l.Sort,
                                Url = l.mapMemberImgUrl(locationManager.member)
                            }).toSafeList()
                };
            }
            else
            {
                return new ManagerResponse(false)
                    .setErrorCode(EkiErrorCode.E023);
            }
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E017);
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            saveUnknowError(e, "Manager/SetLocationImg");
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
