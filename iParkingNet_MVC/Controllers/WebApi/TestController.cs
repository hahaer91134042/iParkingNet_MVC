using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Eki_OCPP;
using OCPP_1_6;
using Eki_LinePayApi_v3;
using Eki_NewebPay;



[RoutePrefix("api/Test")]
public class TestController : BaseApiController
{
    #region 面試用Api
    public class TestResponse
    {
        public TestResponse(bool t)
        {
            code = t ? 200 : 404;
        }
        public int code { get; set; }
        public string msg { get; set; }
        public object info { get; set; }
    }
    private class TestConfig
    {
        public const string key = "This_is_eki_test_123";
    }
    public class TestRequest_2
    {
        public string Key { get; set; }
        public int Type { get; set; }
    }
    private class TitleData
    {
        public string start { get; set; }
        public string end { get; set; }
        public int id { get; set; }
    }
    private class TestTitleData : List<TitleData>
    {
        public TestTitleData()
        {
            var now = DateTime.Now;

            for (var i = 0; i < 10; i++)
            {
                Add(new TitleData
                {
                    start = now.ToString(ApiConfig.DateTimeFormat),
                    end = now.AddMinutes(30).ToString(ApiConfig.DateTimeFormat),
                    id = i
                });
                now = now.AddHours(1);
            }
        }
    }
    private class TestContentData : List<object>
    {
        public TestContentData()
        {
            for (var i = 0; i < 10; i++)
            {
                Add(new
                {
                    id = i,
                    address = $"台北市大安區信義路一段{i}號",
                    carNum = RandomUtil.GetRandomString(5, RandomString.All),
                    amt = RandomUtil.GetRandonNumStr().toDouble()
                });
            }
        }
    }
    [HttpPost]
    [Route("pay_list")]
    public object AppTest(TestRequest_2 testRequest)
    {
        try
        {
            testRequest.saveLog($"test pay_list api be use ");

            if (!testRequest.Key.Equals(TestConfig.key, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException();
            object data = null;
            switch (testRequest.Type)
            {
                case 0:
                    data = new TestTitleData();
                    break;
                case 1:
                    data = new TestContentData();
                    break;
                default:
                    data = new
                    {
                        msg = "本API無這項功能"
                    };
                    break;
            }

            return new TestResponse(true)
            {
                msg = "Success",
                info = data
            };
        }
        catch (ArgumentException)
        {
            return new TestResponse(false)
            {
                msg = "輸入錯誤"
            };
        }
        catch (Exception e)
        {

        }
        return new TestResponse(false)
        {
            msg = "不要亂搞好嗎"
        };
    }
    #endregion

    [HttpGet]
    [Route("ResTest")]
    public object ResTest()
    {
        return new
        {
            Result = ResUtil.GetApiRes("TC", "SmsConfirmMsg")
        };
    }

    [HttpGet]
    [Route("SendMail")]
    public object SendMail()
    {
        try
        {
            var builder = MailConfig.creatBuilder();
            var smtp = builder.from("ekipapaya@eki.com.tw")
                .setSenderName("Test SenderName")
                .useHtmlBody(false)
                .setMsg(new MailMsg().Also(msg =>
                {
                    msg.setTitle("Test Title");
                    msg.AppendMsg("TTEESSSTTT");
                })).build();

            return new
            {
                Result = smtp.SendTo("hahaer91134042@eki.com.tw")
            };
        }
        catch (Exception e)
        {

            return new
            {
                Result=false,
                Msg=e.Message
            };
        }
    }

    [HttpGet]
    [Route("sms")]
    public object SMS()
    {
        try
        {
            var result = EkiSms.create("0986108077").setMsg("TEST").send();

            return new
            {
                Result = result

            };
        }
        catch (Exception e)
        {

            return new
            {
                Result=false,
                Msg=e.ToString()
            };
        }
    }

    [HttpGet]
    [Route("PayInfo")]
    public object PayInfo()
    {
        try
        {
            var pInfo = EkiSql.ppyp.data<MemberPayInfo>(
                "where MemberId=@mid",
                new { mid = 18 });

            return new
            {
                TokenLife=pInfo.neweb.expiryDate().toString()
                
            };
        }
        catch (Exception e)
        {
            return new
            {
                Result = false,
                Msg = e.ToString()
            };
        }
    }

    [HttpGet]
    [Route("CreditTest/{ser}")]
    public object CreditTest(string ser)
    {
        try
        {
            var order = EkiSql.ppyp.data<EkiOrder>(
                "where SerialNumber=@sn",
                new { sn = ser });
            var mPayInfo = EkiSql.ppyp.data<MemberPayInfo>(
                "where MemberId=@mid",
                new { mid = order.MemberId });
            var info = OrderManager.getPayInfo(order);
            var orderNo = OrderPayRecord.NewebPay(order.Id).Also(record => record.Insert()).EkiOrderSerial;

            var creditModel = new NewebPayCreditModel
            {
                MerchantOrderNo = orderNo,
                Amt = info.amt.toInt(),//目前沒有貨幣類別的換算
                ProdDesc = info.desc,
                PayerEmail=order.Member.Mail,
                TokenTerm = order.Member.PhoneNum,
                TokenValue = mPayInfo.neweb.Token
            };

            Log.d($"Credit EkiOrder->{orderNo} PPYPOrder->{order.SerialNumber}");

            var resp = NewebPay.CreditCard().Post(creditModel);


            return new
            {
                Result=true,
                Resp=resp,
                Order=order,
                PayInfo=mPayInfo,
                EkiOrder=orderNo,
                PPYPorder=order.SerialNumber
            };
        }
        catch (Exception e)
        {
            return new
            {
                Result = false,
                Msg = e.ToString()
            };
        }
    }

    [HttpGet]
    [Route("CheckoutProcess/{oid:int}")]
    public object CheckoutProcess(int oid)
    {
        try
        {
            var order = new EkiOrder().Also(o => o.CreatById(oid));

            new OrderCheckOutProcess(order)
            {
                checkSuccess = () => true,
                card4No=()=>"1234"
            }.run();

            return new
            {
                Result = "success"
            };
        }
        catch (Exception e)
        {

            return new
            {
                Msg = e.ToString()
            };
        }
    }

    [HttpGet]
    [Route("LinePay/{oid:int}")]
    public async Task<object> LinePay(int oid)
    {
        try
        {
            var order = new EkiOrder().Also(o => o.CreatById(oid));
            var record = OrderPayRecord.LinePay(order.Id);

            var payin = new LinePayReserve();
            payin.orderId = record.EkiOrderSerial;
            payin.packages.Add(new LinePayReserve.Package().Also(p =>
            {
                p.products.Add(new LinePayReserve.Product()
                {
                    id = order.SerialNumber,
                    name = $"{order.CarNum} 停車費用",
                    price = order.Cost.toInt()
                    //price=1
                });

            }));
            payin.redirectUrls = new LinePayReserve.RedirectUrl
            {
                confirmUrl = EkiLinePay.Config.confirmUrl(order.SerialNumber),
                cancelUrl = EkiLinePay.Config.cancelUrl()
            };

            var client = new LinePay.Client(EkiLinePay.Config);
            //var client = EkiLinePay.Config.getReqClient(payin);

            var result = await client.ReserveAsync(payin);


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
                OrderSerial = order.SerialNumber,
                EkiSerial = payin.orderId,
                TransactionId = result.info.transactionId,
                Result = result
            };
        }
        catch (Exception e)
        {
            return new
            {
                ErrorMsg = e.ToString()
            };
        }
    }


    [HttpGet]
    [Route("OCPP_CheckCP")]
    public object CheckCP()
    {
        var manager = new OrderManager("8475C0E5-BA1A-47F1-B046-DA2A6C4CEEB6");

        var serNum = new List<string>()
        {
            "PAY16463640399271277",
            "PAY16466169291373753"
        };

        var result = manager.checkChargeStatus(serNum);

        //Log.print($"CheckCP result->{result.toJsonString()}");

        return new
        {
            SerNum = serNum,
            Result = result
        };
    }

    [HttpGet]
    [Route("OCPP_TestSend")]
    public object OCPP_GetComp(string serial, string action)
    {
        try
        {
            //EkiOCPP.getCpSchedule("serial123");
            //EkiOCPP.getCpConfig("serial123");
            //EkiOCPP.getCpLoclistver("BCJCB2EM2210004");
            //EkiOCPP.getCpLoclistver(serial);

            //EkiOCPP.getCpSchedule(serial);
            //EkiOCPP.getCpConfig(serial);
            var success = false;

            #region ---測試專區---
            var now = DateTime.Now;
            var rNow = new ReservaNowCall
            {
                expiryDate = now.AddMinutes(15).dateToCpStr()
            };


            var rStart = new RemoteStartTransactionCall(EkiOCPP.Config.EkiAdminIdTag)
            {
                connectorId = 1
            };



            var rSendLoc = new SendLocalListCall(EkiOCPP.Config.OCPP_Version)
            {
                updateType = UpdateType.Full
            }.Also(data =>
            {
                data.localAuthorizationList.Add(LocalAuthorization.creatAccepted(rNow.idTag));
            });


            //cp.auth = OCPP_Status.Authorize.Accepted;
            //cp.idTag = rNow.idTag;
            //EkiOCPP.sendCallAsync(serial, rSendLoc);

            //success= EkiOCPP.sendCall(serial, rStart);

            //EkiOCPP.sendCall(cp.serial,rNow);
            //EkiOCPP.startTranscation(cp.serial, rStart);
            #endregion


            //success = EkiOCPP.sendCall(serial, TriggerMessageCall.Meter);
            var stop = action == "stop";
            if (stop)
                EkiOCPP.stopTranscation(serial);

            var boot = action == "boot";
            if (boot)
                //EkiOCPP.sendCallAsync(serial, TriggerMessageCall.Boot);
                boot = EkiOCPP.sendCall(serial, TriggerMessageCall.Boot);

            var meter = action == "meter";
            if (meter)
                //EkiOCPP.sendCallAsync(serial, TriggerMessageCall.Meter);
                meter = EkiOCPP.sendCall(serial, TriggerMessageCall.Meter);
            var firm = action == "firm";
            if (firm)
                firm = EkiOCPP.sendCall(serial, TriggerMessageCall.Firmware);
            var diag = action == "diag";
            if (diag)
                diag = EkiOCPP.sendCall(serial, TriggerMessageCall.Diagnostic);
            var config = action == "config";
            if (config)
                EkiOCPP.getCpConfig(serial);

            return new
            {
                RemoteStart = action == "start" ? EkiOCPP.sendCall(serial, rStart) : false,
                //TriggleMsg = EkiOCPP.sendCall(serial, TriggerMessageCall.Meter),
                //CacheNum = EkiOCPP.cacheNum,

                RemoteStop = stop,

                Boot = boot,
                Meter = meter,
                Firmware = firm,
                Diagnostic = diag,
                Config = config
            };
        }
        catch (Exception e)
        {
            return new
            {
                Error = e.ToString()
            };
        }
    }

    [HttpGet]
    [Route("ExtendOrder")]
    public object ExtendOrder(string phone)
    {

        var uid = (from m in EkiSql.ppyp.table<Member>()
                   where m.PhoneNum == phone
                   select m.UniqueID.ToString()).FirstOrDefault();

        var manager = new OrderManager(uid);
        //?serial=PAY16474196325452820&end=2022-03-17 05:00:00
        var result = manager.extendOrders(new List<SearchRequest.ExtendOrderEnd>()
        {
            new SearchRequest.ExtendOrderEnd()
            {
                serNum="PAY16492132989640954",
                time="2022-04-11 10:15:00"
            }
        });

        return new
        {
            Success = true,

            Result = result
        };
    }

    [HttpGet]
    [Route("ExtendMsg")]
    public object ExtendMsg()
    {

        var member = new Member().Also(m => m.CreatById(18));

        OrderExtendContent.load(new EkiOrder()
        {
            SerialNumber = "PAY_Test"
        }.Also(o =>
        {
            o.ReservaTime.EndTime = DateTime.Now;
        })).sendTo(member);

        return new
        {
            Success = true
        };
    }

    [HttpGet]
    [Route("OrderTest")]
    public object OrderTest(SearchRequest request)
    {

        return (from s in request.serNum
                join o in EkiSql.ppyp.table<EkiOrder>() on s equals o.SerialNumber
                where o.beEnable
                where o.StatusEnum == OrderStatus.Reserved
                select new
                {
                    SerNum = o.SerialNumber

                });
    }

    [HttpGet]
    [Route("OrderRecord")]
    public object OrderRecord()
    {
        try
        {
            //OrderPayRecord.creat(2039).Also(record =>
            //{
            //    record.saveLog("OrderPayRecord");
            //    record.Insert();
            //});

            var order = (from o in GetTable<EkiOrder>()
                         where o.Id == 2039
                         select o).FirstOrDefault();

            var result = (from r in GetTable<OrderPayRecord>()
                          where r.OrderId == order.Id
                          join pr in GetTable<NewebPayReturn>() on r.OrderId equals pr.OrderId into NewebReturn
                          where !NewebReturn.Any(nr => nr.MerchantOrderNo == r.EkiOrderSerial)
                          orderby r.cDate descending
                          select r).FirstOrDefault();


            return new
            {
                Order = order,
                Result = result
            };
        }
        catch (Exception e)
        {
            return new
            {
                Error = e.Message,
                Exception = e
            };
        }
    }

    [HttpGet]
    [Route("LogTest")]
    public object LogTest(string msg)
    {
        try
        {
            Log.d($"test log->{msg}");

            return new
            {
                Success = true,
                Msg = msg
            };
        }
        catch (Exception e)
        {
            return new
            {
                Error = e.ToString(),
                Path = WebUtil.getWebBaseDirectoryPath() + "log"
            };
        }
    }

    [HttpGet]
    [Route("ResTest")]
    public object ResTest(string key)
    {
        try
        {
            var value = ResUtil.GetApiRes(LanguageFamily.TC, key);
            var c = new CheckOutContent()
            {
                OrderSerNum = "PAY123456",
                StartTime = DateTime.Now.toString(),
                EndTime = DateTime.Now.AddHours(1).toString()
            };

            var type = c.GetType();
            var typeInterface = typeof(IiosPushNotify);

            return new
            {
                Key = key,
                Value = value,
                Type = type,
                Interfactype = typeInterface,
                A1 = type.IsAssignableFrom(typeInterface),
                A2 = typeInterface.IsAssignableFrom(type),
                ExtTest = c.hasInterface<IiosPushNotify>()
            };
        }
        catch (Exception e)
        {
            return new
            {
                Error = e.Message
            };
        }
    }

    [HttpPost]
    [Route("StringTest")]
    [JwtAuthActionFilter]
    public object stringTest(MemberEditRequest request)
    {
        request.saveLog(request.info.nickName);
        //request.info.nickName.saveLog("string");

        var cmd = SqlCmd.InsertTo<ErrorLog>.ObjTable(new ErrorLog
        {
            Input = null,
            Exception = "",
            Msg = "123",
            Ip = WebUtil.GetUserIP()
        });

        var ilCmd = SqlCmd.InsertTo<ErrorLog>.ObjTable(new List<ErrorLog>
        {
            new ErrorLog
        {
            Input = null,
            Exception = "",
            Msg = "123",
            Ip = WebUtil.GetUserIP()
        },new ErrorLog
        {
            Input = null,
            Exception = "",
            Msg = "456",
            Ip = WebUtil.GetUserIP()
        }
        });

        var uCmd = SqlCmd.Update<ErrorLog>.ObjTableById(new ErrorLog
        {
            Id = 1950,
            Input = null,
            Exception = "",
            Msg = "123",
            Ip = WebUtil.GetUserIP()
        });
        //var uCmd2 = SqlCmd.Update<ErrorLog>.ObjTestTableById(new ErrorLog
        //{
        //    Id = 1950,
        //    Input = null,
        //    Exception = "",
        //    Msg = "123",
        //    Ip = WebUtil.GetUserIP()
        //});
        var ulComd = SqlCmd.Update<ErrorLog>.TableByList(new List<ErrorLog>
        {
            new ErrorLog
        {
            Id=1950,
            Input = null,
            Exception = "",
            Msg = "123",
            Ip = WebUtil.GetUserIP()
        },new ErrorLog
        {
            Id=1951,
            Input = null,
            Exception = "",
            Msg = "123",
            Ip = WebUtil.GetUserIP()
        }
        });
        //cmd.saveLog("Test Cmd");

        var raw = request;
        var cleanResult = TextUtil.cleanHtmlFragmentXss(request.info.nickName);


        return new
        {
            Raw = raw,
            InsertCmd = cmd,
            InsertListCmd = ilCmd,
            UpdateCmd = uCmd,
            //UpdateTestCmd = uCmd2,
            UpdateListCmd = ulComd,
            DBNull = DBNull.Value.ToString(),
            CleanResult = cleanResult
        };
    }

    [HttpGet]
    [Route("TestFcmToken")]
    public object TestFcmToken(string token, int type)
    {
        try
        {
            var now = DateTime.Now;
            var member = new Member()
            {
                PushToken = token,
                MobileType = type
            };

            new GetOrderContent()
            {
                Name = "這是測試使用",
                Start = now.ToString(ApiConfig.DateTimeFormat),
                End = now.ToString(ApiConfig.DateTimeFormat),
                CarNum = "AA123"

            }.sendTo(member);


            return new
            {
                Time = now,
                Result = true,
                Token = token
            };
        }
        catch (Exception e)
        {
            return new
            {
                Result = false,
                Error = e.ToString()
            };
        }

    }


    [HttpGet]
    [Route("DateTest")]
    public object DateTest()
    {
        try
        {
            var time = "2021-03-22 24:00:00".parse24Hour();

            return new
            {
                Time = time,
            };
        }
        catch (Exception e)
        {
            return e;
        }

    }

    [HttpPost]
    [Route("SearchTimeTest")]
    public object SearchTimeTest(SearchRequest request)
    {
        try
        {
            var manager = new LocationOwnerManager(new Member().Also(m => m.CreatById(18)));


            return manager.getLocMulctOrder(request);
        }
        catch (InputFormatException)
        {
            var list = new List<object>();

            request.times.ForEach(time =>
            {
                var bStart = false;
                var start = time.start.toDateTime(ApiConfig.DateFormat, b => { bStart = b; });
                var bEnd = false;
                var end = time.end.toDateTime(ApiConfig.DateFormat, b => { bEnd = b; });

                list.Add(new
                {
                    bStart,
                    start,
                    bEnd,
                    end
                });
            });
            return list;
        }
        catch (Exception e)
        {
            return e.ToString();
        }

    }



    [HttpGet]
    [Route("FixImg")]
    public object FixImg()
    {
        try
        {
            var locImgList = (from loc in GetTable<Location>()
                              select new LocImg
                              {
                                  LocationId = loc.Id,
                                  Sort = 1,
                                  Img = loc.Info.Img
                              }).toSafeList();

            locImgList.ForEach(img =>
            {
                img.Insert();
            });


            return new
            {
                Success = true
            };
        }
        catch (Exception)
        {

        }

        return new
        {
            Success = false
        };
    }

    [HttpGet]
    [Route("VerifyTest")]
    public object VerifyTest()
    {
        try
        {

            return new
            {
                Success = true,
                Result = new Verify
                {
                    typeEnum = VerifyType.Order,
                    statusEnum = VerifyStatus.Processing,
                    ItemId = 11

                }.Insert()
            };
            //return new
            //{
            //    Success = true,
            //    Result = (from v in GetTable<Verify>()
            //              select v).First().Update()
            //};
        }
        catch (Exception e)
        {
            return new
            {
                Success = false,
                Msg = e.ToString()
            };
        }
    }

    [HttpGet]
    [Route("Invoice")]
    public object InvoiceAsync()
    {
        try
        {
            var data = new NewebPayInvoiceModel
            {
                TransNum = "19112814071241556",
                MerchantOrderNo = "PAY123456788",
                Category = NewebPayInvoice.Category_B2C,
                BuyerName = "Hill",
                //BuyerUBN = "123456",
                BuyerAddress = "台北市大安區信義路",
                BuyerEmail = "hahaer91134042@eki.com.tw",
                //CarrierType=PayConfig.Invoice.CarrierType.ezPay,
                //CarrierNum="/JW4EW3B",
                PrintFlag = NewebPayInvoice.PrintFlag_Y,

                Amt = 95,
                TaxAmt = 5,
                TotalAmt = 100,
                Comment = "123456"
            };
            var config = PayConfig.Invoice.config();
            var parser = NewebPayInfoParser.Parse(data);
            var info = config.EncryptAES256(parser.GetInfo());

            //var client = new HttpClient();
            //client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            //var response = await client.PostAsync("http://iparkingnet.eki.com.tw/api/test/posttest", null);
            //var responseString = await response.Content.ReadAsStringAsync();

            var response = NewebPay.Invo().Post(data);

            return new
            {
                Raw = data,
                RawInfo = parser.GetInfo(),
                Encrypto = info,
                PostTest = response
            };
        }
        catch (Exception e)
        {
            return e;
        }
    }

    [HttpGet]
    [Route("SysManager")]
    public object Notify()
    {
        try
        {
            var cmd = SqlCmd.Get<SysManager>.TableData();

            var sm = (from s in GetTable<SysManager>()
                      select s);


            return new
            {
                Cmd = cmd,
                SysManager = sm
            };
        }
        catch (Exception e)
        {
            return e;
        }

    }

    [HttpGet]
    [Route("SocketTest")]
    public object SocketTest()
    {

        var member = (from m in EkiSql.ppyp.table<Member>()
                      where m.PhoneNum == "0912345679"
                      select m).First();

        member.sendPushMsg(new ManagerOrderCancelContent()
        {
            Order = new EkiOrderResponse(),
            Discount = new DiscountResponse()
        });

        return new
        {
            Sucess = true,
            Member = member
        };
    }

    [HttpGet]
    [Route("RoundTest")]
    public object RoundTest(double input)
    {
        return new
        {
            Result = Math.Round(input, 1, MidpointRounding.AwayFromZero)
        };
    }

    [HttpGet]
    [Route("ActionTest")]
    public object ActionTest()
    {
        var request = new SearchRequest
        {
            serNum = new List<string> { "LovePPYP", "AAAAss" }
        };
        var now = DateTime.Now;
        var result = (from serial in request.serNum
                      join a in GetTable<EkiAction>() on serial.ToUpper() equals a.Code.ToUpper() into Action
                      join c in GetTable<EkiCheckOut>() on 18 equals c.MemberId into CheckOut
                      select new
                      {
                          Serial = serial,
                          Action = (from a in Action
                                    where !CheckOut.Any(o => o.ActionId == a.Id)
                                    where a.beEnable
                                    where a.TimeLimit ? a.StartTime <= now && now <= a.EndTime : true
                                    select a).FirstOrDefault()?.convertToResponse()
                      }).toSafeList();

        return new
        {
            result
        };
    }

    [HttpGet]
    [Route("CheckOutTest")]
    public object ActionCode()
    {
        try
        {
            var member = (from m in GetTable<Member>()
                          where m.Id == 18
                          select m).FirstOrDefault();
            var order = (from o in GetTable<EkiOrder>()
                         where o.SerialNumber == "PAY15827992398545085"
                         select o).FirstOrDefault();
            var calter = OrderManager.OrderCalculater.init(order, new CheckOutRequest
            {
                number = "PAY15827992398545085",
                date = "2020-03-02 09:30:00",
                action = "",
                discount = ""
            });

            var result = calter.calCheckOut(null);


            //result.consume.Insert();


            return new
            {
                Result = result
            };
        }
        catch (Exception e)
        {
            return e;
        }
    }

    [HttpGet]
    [Route("BankTest")]
    public object BankTest()
    {
        var bankList = (from b in GetTable<BankInfo>()
                        select b).toSafeList();

        return new
        {
            BankList = bankList,
            BankDecode = bankList.Select(b => b.bankDecode).toSafeList()
        };
    }

    [HttpGet]
    [Route("CancelOpenTest")]
    public object CancelOpenTest()
    {
        var now = DateTime.Now;
        var order = (from o in GetTable<EkiOrder>()
                     where o.SerialNumber == "PAY15874631082418794"
                     select o).FirstOrDefault();




        return new
        {
            Order = order,
            CompensationAmt = order.calCompensation(now)
        };
    }

    [HttpPost]
    [Route("AddOpenTest")]
    public object AddOpenTest(EditOpenSetRequest request)
    {
        var auth = getAuthObj();
        var locationManager = auth.creatLocationOwnerManager();

        var location = (from m in GetTable<Location>()
                        where m.Id == request.id
                        select m).First();
        var addList = request.openSet.convertToDbObjList<OpenTime>();

        var openCheck = new OpenCheck(location.ReservaConfig.OpenSet);

        var any = (from o in addList
                   where openCheck.anyOverLap(o)
                   select o).Count();
        try
        {
            return new
            {
                AddList = addList,
                CheckList = openCheck.openList,
                OverLapCount = any,
                Obj1 = openCheck.anyOverLap(addList[0]),
                SameWeek1 = (from o in openCheck.openList
                             where o.weekEnum == addList[0].weekEnum
                             select o),
                Between1 = (from o in openCheck.openList
                            where o.weekEnum == addList[0].weekEnum
                            select o).Let(list =>
                            {

                                var other = addList[0].getStartTime();

                                return (from o in list
                                        where o.between(other)
                                        select o);
                            }),
                obj2 = openCheck.anyOverLap(addList[1]),
                obj2Start = addList[1].getStartTime(),
                SameWeek2 = (from o in openCheck.openList
                             where o.weekEnum == addList[1].weekEnum
                             select o),

                Between2 = (from o in openCheck.openList
                            where o.weekEnum == addList[1].weekEnum
                            select o).First().Let(open =>
                            {

                                var other = addList[1].getStartTime();
                                //把現在這個週期時間轉成比較時間日期一樣
                                var newOpen = new OpenTime().Also(o =>
                                {
                                    o.Date = other.ToString(ApiConfig.DateFormat);
                                    o.StartTime = open.StartTime;
                                    o.EndTime = open.EndTime;
                                });
                                return newOpen.getStartTime() <= other && other <= newOpen.getEndTime();

                            }),

                TimeRange = addList[1].getStartTime()
            };
        }
        catch (Exception e)
        {
            return e;
        }

    }
    private class ManagerResponse : ResponseInfoModel<object>
    {
        public ManagerResponse(bool successful) : base(successful)
        {
        }
    }

    [HttpGet]
    [Route("SendAsync")]
    public object SendAsync()
    {
        try
        {
            var start = DateTime.Now;

            var test = new AsyncTest();

            Log.d("test start");
            var result = test.start();

            var end = DateTime.Now;

            //var result= new List<Task> { test.start() };

            //while (result.Count > 0)
            //{
            //    var rTask = await Task.WhenAny(result);



            //}



            return new
            {
                Start = start.toString(),
                End = end.toString()
            };
        }
        catch (Exception e)
        {
            return new
            {
                Error = e.ToString()
            };
        }
    }

    private class AsyncTest
    {

        public Task<DateTime> start()
        {

            return Task.Run(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
                var now = DateTime.Now;
                Log.d($"task start  {now.toString()}");
                var member = (from m in EkiSql.ppyp.table<Member>()
                              where m.PhoneNum == "0987987487"
                              select m).First();
                Log.d($"member->{member.toJsonString()}");
                try
                {
                    member.sendPushMsg(new CheckOutContent()
                    {
                        OrderSerNum = "PAY123456",
                        StartTime = now.toString(),
                        EndTime = now.AddHours(1).toString()
                    });
                    Log.d("send push finish");
                }
                catch (Exception e)
                {
                    Log.d($"send push error ->{e.ToString()}");
                }

                return now;
            });
        }


    }

    [HttpGet]
    [Route("SendMsg")]
    public object SendMsg(string phone)
    {
        try
        {
            var member = (from m in EkiSql.ppyp.table<Member>()
                          where m.PhoneNum == phone
                          select m).First();

            var content = new CheckOutContent()
            {
                OrderSerNum = "PAY123456",
                StartTime = DateTime.Now.toString(),
                EndTime = DateTime.Now.AddHours(1).toString()
            };

            member.sendPushMsg(content);

            return new
            {
                Success = true,
                Token = member.PushToken,
                //SendMulti=FcmManager.New().SendMultiClient(new List<IFcmSet>() {member},content)
            };
        }
        catch (Exception e)
        {
            return e;
        }
    }

    [HttpGet]
    [Route("DiscountTest")]
    public object DiscountTest()
    {
        var ranStr = "";
        var secret = ApiConfig.Discount.Secret;
        int ans;
        var rule = new DiscountCodeRule();
        do
        {
            try
            {
                ranStr = RandomUtil.GetRandomString(ApiConfig.Discount.CodeLength, RandomString.All) +
                    RandomUtil.GetRandomString(ApiConfig.Discount.CodeFullLength - ApiConfig.Discount.CodeLength, RandomString.All);
                ans = ((TextUtil.IntASC(ranStr.Substring(0, ApiConfig.Discount.CodeLength)) / TextUtil.IntASC(secret)) % 10).toInt();
            }
            catch (Exception)
            {
                ans = -1;
            }

        } while (!rule.isInRule(ranStr));

        var testCode = SerialNumUtil.DiscountSerialNum();
        return new
        {
            RanStr = ranStr,
            SubRan = ranStr.Substring(0, ApiConfig.Discount.CodeLength),
            Secret = secret,
            RanAsc = TextUtil.IntASC(ranStr),
            SecretAsc = TextUtil.IntASC(secret),
            Cal = (TextUtil.IntASC(ranStr.Substring(0, ApiConfig.Discount.CodeLength)) / TextUtil.IntASC(secret)) % 10,
            Ans = ans,
            IsInRule = rule.isInRule(ranStr),
            TestCode = testCode,
            TestValid = rule.isInRule(testCode)
        };
    }

    [HttpGet]
    [Route("Time")]
    public object TimeTest()
    {
        try
        {
            //var input = new OpenTime()
            //{
            //    Date = "2020-03-27",
            //    StartTime = start,
            //    EndTime = end
            //};
            var ori = new OpenTime()
            {
                Date = "2020-04-29",
                StartTime = "00:00:00",
                EndTime = "02:00:00"
            };

            var other = new OpenTime()
            {
                Date = "2020-04-29",
                StartTime = "00:30:00",
                EndTime = "02:30:00"
            };
            var OriStart = ori.getStartTime();
            var OriEnd = ori.getEndTime();

            var OtherStart = other.getStartTime();
            var OtherEnd = other.getEndTime();


            //return input ==ori;

            //return group;
            return new
            {
                OriStart,
                OriEnd,

                OtherStart,
                OtherEnd,

                Test1 = OriStart <= OtherStart,
                Test2 = OtherStart <= OriEnd,

                OriBetweenStart = ori.between(other.getStartTime()),
                OriBetweenEnd = ori.between(other.getEndTime()),

                OtherBetweenStart = other.between(ori.getStartTime()),
                OtherBetweenEnd = other.between(ori.getEndTime()),

                OriOverLap = ori.overlap(other),
                OtherOverLap = other.overlap(ori)
            };
        }
        catch (Exception e)
        {
            return e;
        }
    }

    DateTime parse24Hour(string input)
    {
        var wrapped = Regex.Replace(input, @"24:(\d\d:\d\d)$", "00:$1");
        var res = DateTime.Parse(wrapped);
        return wrapped != input
            ? res.AddDays(1)
            : res;
    }

    [HttpGet]
    [Route("DbTest")]
    public object DbTest()
    {
        try
        {

            return new
            {
                Success = true,

            };
        }
        catch (Exception e)
        {
            return e;
        }
    }

    [HttpPost]
    [Route("SendFcm")]
    public object SendFcm(SendFcmMsg value)
    {
        try
        {
            using (var manager = FcmManager.New())
            {
                //var result = manager.SendTo(value.token, new FcmNotification()
                //{
                //    title = "Test",
                //    body = value.msg
                //});
                //var result = manager.SendTo(value.token.FirstOrDefault(), new BroadCastMsg<object>().Also(msg =>
                //{
                //    msg.Method = EkiBroadCastMethod.GetCheckOut.Name;
                //    msg.Content = new
                //    {
                //        Msg1 = "Test1",
                //        Msg2 = value.msg
                //    };
                //}));

                //var result = manager.SendMultiClient(value.token, new BroadCastMsg<IBroadCastContent>().Also(msg =>
                //  {
                //      msg.Content = new TestContent(value.msg);
                //  }));


                return new
                {
                    Success = true,
                    Result = false
                };
            }
        }
        catch (Exception e)
        {
            return new
            {
                e
            };
        }
    }
    private class TestContent : IFcmMsg
    {
        public TestContent(string msg)
        {
            Msg2 = msg;
        }
        public string fcmMethod() => EkiBroadCastMethod.GetCheckOut.Name;
        public string Msg1 = "Test1";
        public string Msg2 = "";
    }
    public class SendFcmMsg
    {
        public List<string> token { get; set; }
        public string msg { get; set; }
    }


    [HttpGet]
    [Route("LinqTest")]
    public object LinqTest()
    {
        var idList = new List<int>() { 13, 14, 15 };

        //TableParaser.ConvertQueryByRowName<Location>(sqlHelper.query(SqlCmd.Get<Location>.TableData()))

        var locations = (from id in idList.AsEnumerable()
                         join loc in EkiSql.ppyp.table<Location>() on id equals loc.Id
                         select loc).AsQueryable();//先找出使用者要預約的位置資料

        //ActionContext.Request.GetQueryNameValuePairs()

        return new
        {
            Location = locations
        };
    }

    [HttpGet]
    [Route("TimeSpanTest")]
    public object Test()
    {
        var time1 = DateTime.Parse("2019-07-30");
        //var time2 = DateTime.Parse("2019-07-19 13:45:00");

        return new
        {
            TimeStamp = TimeUtil.toStamp(time1)
        };
    }

    [HttpGet]
    [Route("SocketTest")]
    public object SocketTest(string msg)
    {
        var broadcastSocket = BroadcastSocket.Instance;
        broadcastSocket.SendAll($"From Api->{msg}  time->{DateTime.Now}");
        return new
        {
            Success = broadcastSocket != null,
            Msg = msg
        };
    }

    [HttpPost]
    [Route("CryptoTest")]
    public object CryptionTest(TestRequest request)
    {
        var hash = SecurityBuilder.CreateHashCode(EncryptFormat.SHA1, request.key, ApiConfig.JwtSecret);
        var data = SecurityBuilder.DecryptTextByAES(request.content, hash);
        return new
        {
            Result = data
        };
    }
    public class TestRequest
    {
        public string key { get; set; }
        public string content { get; set; }
    }

    [HttpGet]
    [Route("SecurityTest")]
    public object SecurityTest()
    {
        try
        {
            //var sBuilder = SecurityBuilder.New();
            //var salt = SecurityBuilder.CreateSaltKey(5);

            var request = new BankDecode()
            {
                name = "987media company",
                isPerson = false,
                serial = "987987123",
                code = "001",
                account = "12344487"
            };

            var aesEncode = EkiEncoder.AES;
            var crypto = aesEncode.encode(request);

            var hash = request.creatHash(crypto.publicKey, request.hashSet().secret());

            //var encode = sBuilder.EncryptTextBy3DES(ObjToText(request), hash);
            //var encode = request.encryptByAES(hash);
            //var decode = encode.decryptByAES<CreditDecode>( hash);

            //var testBank = (from b in GetTable<BankInfo>()
            //                where b.MemberId == 18
            //                select b).First();

            return new
            {
                Input = request,

                Request = new BankRequest()
                {
                    key = crypto.publicKey,
                    content = crypto.cipher
                },
                Decode = aesEncode.decode<BankDecode>(crypto.cipher, crypto.publicKey),

                Hash = hash,
                EncryptKey = hash.Substring(0, 16),
                EncryptIV = hash.Substring(8, 8),
                HashIV_ASCII = Encoding.ASCII.GetBytes(hash.Substring(8, 8))
                //BankDecodeTest=testBank
            };
        }
        catch (Exception e)
        {
            return e;
        }
    }

    [HttpGet]
    [Route("RSA")]
    public object RSA()
    {
        //var text = ObjToText(new CreditDecode()
        //{
        //    category = 0,
        //    cardNum = "1234-7788-5566",
        //    limitDate = "2020-06",
        //    check = "123",
        //    firstName = "Chia-Hung",
        //    lastName = "Ku"
        //});

        var text = new CreditDecode()
        {
            category = 0,
            cardNum = "1234-7788-5566",
            limitDate = "2020-06",
            check = "123",
            firstName = "Chia-Hung",
            lastName = "Ku"
        }.toJsonString();

        var keys = GenerateRSAKeys();
        var publicKey = keys.Item1;
        var privateKey = keys.Item2;

        var encode = Encrypt(publicKey, text);

        return new
        {
            RSAkey = keys,
            PublicKey = publicKey,
            PrivateKey = privateKey,
            Text = text,
            Encode = encode,
            Decode = Decrypt(privateKey, encode)
        };
    }

    private Tuple<string, string> GenerateRSAKeys()
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

        var publicKey = rsa.ToXmlString(false);
        var privateKey = rsa.ToXmlString(true);

        return Tuple.Create<string, string>(publicKey, privateKey);
    }

    private string Encrypt(string publicKey, string content)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(publicKey);

        var encryptString = Convert.ToBase64String(rsa.Encrypt(Encoding.UTF8.GetBytes(content), false));

        return encryptString;
    }
    private string Decrypt(string privateKey, string encryptedContent)
    {
        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
        rsa.FromXmlString(privateKey);

        var decryptString = Encoding.UTF8.GetString(rsa.Decrypt(Convert.FromBase64String(encryptedContent), false));

        return decryptString;
    }

}
