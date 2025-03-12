using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

[RoutePrefix("api/Order")]
public class OrderController : BaseApiController
{

    #region CreditAgree
    [HttpPost]
    [Route("CreditAgree")]
    [JwtAuthActionFilter]
    public object CreditAgree(SearchRequest req)
    {
        try
        {
            if (req.serial.isNullOrEmpty())
                throw new ArgumentNullException();

            var order = EkiSql.ppyp.data<EkiOrder>(
                        "where SerialNumber=@ser",
                        new { ser = req.serial });
            if (order == null)
                throw new ArgumentNullException();

            //var orderManager = new OrderManager("8475C0E5-BA1A-47F1-B046-DA2A6C4CEEB6");
            var orderManager = new OrderManager(getAuthObj().user);

            var newebReturn = orderManager.payCreditAgree(order);

            return new OrderResponse(true)
            {
                info = new
                {
                    Serial=order.SerialNumber,
                    Amt=newebReturn.Result.Amt,
                    Card4=newebReturn.Result.Card4No,
                    Time= DateTime.Now.toStamp().ToString(),
                    TokenLife=newebReturn.Result.TokenLife
                }
            };
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
            Log.e("CreditAgree Error", e);
            //e.saveLog("CreditAgreeError", "OrderController");
        }
        return ResponseError();
    }
    #endregion

    #region LinePay
    [HttpPost]
    [Route("LinePay")]
    [JwtAuthActionFilter]
    public async Task<object> LinePay(SearchRequest req)
    {
        try
        {
            if (req.serial.isNullOrEmpty())
                throw new ArgumentNullException();

            var order = EkiSql.ppyp.data<EkiOrder>(
                "where SerialNumber=@ser",
                new { ser = req.serial });

            if (order == null)
                throw new ArgumentNullException();

            var orderManager = new OrderManager(getAuthObj().user);
            //var orderManager = new OrderManager("8475C0E5-BA1A-47F1-B046-DA2A6C4CEEB6");

            return new OrderResponse(true)
            {
                info = await orderManager.linePayReserve(order)
            };
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

        }
        return ResponseError();
    }
    #endregion

    #region CheckCharge
    [HttpPost]
    [Route("CheckCharge")]
    [JwtAuthActionFilter]
    public object CheckCharge(SearchRequest request)
    {
        try
        {
            if (request.isSerNumEmpty())
                throw new ArgumentException();

            var orderManager = new OrderManager(getAuthObj().user);

            return new OrderResponse(true)
            {
                info=orderManager.checkChargeStatus(request.serNum)
            };
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception)
        {

        }
        return ResponseError();
    }
    #endregion
    #region StartCharge
    [HttpPost]
    [Route("StartCharge")]
    [JwtAuthActionFilter]
    public object StartCharge(SearchRequest request)
    {
        try
        {            
            if (request.isSerNumEmpty())
                throw new ArgumentException();

            var orderManager = new OrderManager(getAuthObj().user);

            return new OrderResponse(true)
            {
                info = orderManager.startCharge(request.serNum)
            };

        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch(Exception e)
        {

        }
        return ResponseError();
    }
    #endregion

    [HttpPost]
    [Route("SetEnd")]
    [JwtAuthActionFilter]
    public object SetEnd(SearchRequest request)
    {
        try
        {
            var data = request.getData<List<SearchRequest.ExtendOrderEnd>>();
            if (data.Count < 1)
                throw new ArgumentNullException();
            var orderManager = new OrderManager(getAuthObj().user);

            var result = orderManager.setOrderEnd(data);

            return new OrderResponse(true)
            {
                info = result
            };
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception)
        {

        }
        return ResponseError();
    }

    #region ---ExtendTime---
    [HttpPost]
    [Route("ExtendTime")]
    [JwtAuthActionFilter]
    public object ExtendTime(SearchRequest request)
    {
        try
        {
            var data = request.getData<List<SearchRequest.ExtendOrderEnd>>();
            if (data.Count < 1)
                throw new ArgumentNullException();

            var orderManager = new OrderManager(getAuthObj().user);
            var result = orderManager.extendOrders(data);

            return new OrderResponse(true)
            {
                info=result
            };
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch(Exception e)
        {

        }
        return ResponseError();
    }
    #endregion

    #region ---CheckOut---
    [HttpPost]
    [Route("CheckOut")]
    [JwtAuthActionFilter]
    public object CheckOut()
    {
        try
        {
            //if (!this.formFileContain(RequestFlag.Body.Img) || !this.formDataContain(RequestFlag.Body.Info))
            //    throw new ArgumentNullException();
            //目前可以不必要傳入圖片
            if ( !this.formDataContain(RequestFlag.Body.Info))
                throw new ArgumentNullException();

            var auth = getAuthObj();            

            var orderManager = new OrderManager(auth.user);
            var postImg = this.getPostImg(RequestFlag.Body.Img);
            var checkOut = this.getPostObj<CheckOutRequest>(RequestFlag.Body.Info);

            //checkOut.saveLog("checkOut data",this);

            if (!checkOut.isValid())
                throw new ArgumentException();


            var result = orderManager.OrderCheckOut(postImg, checkOut);


            //postImg.saveTo($"~{string.Format(DirPath.Order, auth.user)}");
            return new OrderResponse(true)
            {
                info = new
                {
                    result.Url
                }
            };
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E013);
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E014);
        }
        catch (ArgumentException e)
        {
            //return e;
            //var checkOut = getPostObj<CheckOutRequest>(RequestFlag.Body.Info);
            //saveUnknowError(e, checkOut.toJsonString());
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception e)
        {
            var checkOut = this.getPostObj<CheckOutRequest>(RequestFlag.Body.Info);
            saveUnknowError(e, checkOut.toJsonString());
            //return e;
        }
        return ResponseError();
    }
    #endregion
    #region ---CancelImg---
    [HttpPost]
    [Route("CancelImg")]
    [JwtAuthActionFilter]
    public object CancelImg()
    {
        try
        {
            var auth = getAuthObj();
            var orderManager = new OrderManager(auth.user);

            if (orderManager.setOrderCancelImg())
            {
                return new OrderNoInfoResponse(true);
            }
            else
            {
                return new OrderNoInfoResponse(false);
            }
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E023);
        }        
        catch(Exception e)
        {
            saveUnknowError(e,"OrderCancelImg");
        }
        return ResponseError();
    }
    #endregion
    #region ---Cancel---
    [HttpPost]
    [Route("Cancel")]
    [JwtAuthActionFilter]//之後要計算手續費
    public object Cancel(SearchRequest request)
    {
        try
        {
            if (request.isSerNumEmpty())
                throw new ArgumentNullException();

            if (request.serNum.Count() > ApiConfig.OrderCancelLimit)
                throw new ArgumentOutOfRangeException();

            var auth = getAuthObj();
            var orderManager = new OrderManager(auth.user);

            var result = orderManager.CancelOrder(request);


            return new OrderResponse(true)
            {
                info = result
            };
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E006);
        }
        catch (OutOfDateTimeException)
        {
            return ResponseError(EkiErrorCode.E002);
        }
        catch (ArgumentOutOfRangeException)
        {
            return ResponseError(EkiErrorCode.E016);
        }
        catch (OutOfLimitException)
        {
            return ResponseError(EkiErrorCode.E019);
        }
        catch (Exception e)
        {
            saveUnknowError(e, request.toJsonString());
            //return e;
        }
        return ResponseError();
    }
    #endregion

    #region ---Reservation---
    [HttpPost]
    [Route("Add")]
    [JwtAuthActionFilter]
    public object Add(ReservaRequest request)
    {
        try
        {
            //request.saveLog("Hill->", this);

            if (!request.isValid())
                throw new InputFormatException();

            var auth = getAuthObj();

            var orderManager = new OrderManager(auth.user);

            var orderList = orderManager.CreatReservaOrder(request.times.convertToDbObjList<ReservedTime>());
            var dbList = new DbObjList<EkiOrder>(orderList);

            if (dbList.InsertByObj())
            {
                var members = EkiSql.ppyp.table<Member>();
                var locs = EkiSql.ppyp.table<Location>();
                var resultList = new List<AddResult>();
                request.times.ForEach(data =>
                {
                    //找出成立的訂單跟request相匹配的
                    var order = (from o in GetTable<EkiOrder>()
                                 where o.LocationId == data.loc
                                 // where o.ReservaTime.Date.toStamp(ApiConfig.DateFormat) == data.date.toStamp(ApiConfig.DateFormat) 
                                 where o.ReservaTime.StartTime == data.start.toDateTime() &&
                                 o.ReservaTime.EndTime == data.end.toDateTime()
                                 where orderList.Any(r => r.SerialNumber == o.SerialNumber)//保證是新生的
                                 select o).FirstOrDefault();
                    //找不到 代表這筆預約的request失敗
                    if (order == null)
                    {
                        var loadManager = new LoadManager();
                        var reserva = loadManager.getReservaTimeFrom(new SearchRequest() { id = new List<int>() { data.loc } })[0];

                        resultList.Add(new AddResult()
                        {
                            Time = RemoveObjAttr(data, "week", "serNum"),
                            LoadData=reserva,
                            Success=false
                        });
                    }
                    else//找到 代表這筆有預約成功
                    {
                        //訂單成立會送FCM到地主手機
                        (from l in locs
                         where l.Id == order.LocationId
                         select l).FirstOrDefault().notNull(loc =>
                         {
                             (from m in members
                              where m.Id == loc.MemberId
                              select m).FirstOrDefault().notNull(member =>
                              {
                                  member.sendGetOrderMsg(order, loc);
                              });                             
                         });

                        resultList.Add(new AddResult()
                        {
                            Time = RemoveObjAttr(data, "week", "serNum"),
                            Order = order.convertToResponse(),
                            Success = true
                        });
                    }
                });

                return new OrderResponse(true)
                {
                    info=resultList
                };
            }
            else
            {
                return ResponseError();
            }
        }
        catch (OrderNoPayException)
        {
            return ResponseError(EkiErrorCode.E020);
        }
        catch (InputFormatException e)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (TimeOverlapException)
        {
            return ResponseError(EkiErrorCode.E021);
        }
        catch (Exception e)
        {
            saveUnknowError(e, request);
            //return e;
        }
        return ResponseError();
    }
    #endregion

    public class AddResult
    {
        public object Time { get; set; }
        public object Order { get; set; }
        public object LoadData { get; set; }
        public bool Success{ get; set; }        
    }

    public class OrderNoInfoResponse : ResponseAbstractModel
    {
        public OrderNoInfoResponse(bool successful) : base(successful)
        {
        }
    }
    public class OrderResponse : ResponseInfoModel<object>
    {
        public OrderResponse(bool successful) : base(successful)
        {
        }
    }
}
