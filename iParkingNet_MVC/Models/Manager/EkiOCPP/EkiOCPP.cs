using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DevLibs;
using OCPP_1_6;
using Fleck;
using System.Timers;

/// <summary>
/// EkiOCPP 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class EkiOCPP : BaseManager, IOCPP_SocketEvent
    {
        private static EkiOCPP ekiOCPP;
        public static List<ChargePoint> cpList { get => OCPPsocket.cpList; }

        private OCPPsocket ocppSocket;
        private CpCallSort callSorts = new CpCallSort();
        private CleanErrorCpTimer cleanTimer = new CleanErrorCpTimer();

        private static SendMsgCache sendCache = new SendMsgCache();
        //private Timer timer=new Timer();
        public static int cacheNum { get => sendCache.Count; }
        private EkiOCPP(OCPPsocket s)
        {
            ocppSocket = s;
            ocppSocket.addEvent(this);
            //ocppSocket.addEvent(new EkiOCPP_Event(ocppSocket));

            //ocppSocket.socketEvents = new EkiOCPP_Event(ocppSocket);

            cleanTimer.run();

        }

        #region 靜態方法
        public static OCPPsocket start()
        {
            if (ekiOCPP == null)
            {
                ekiOCPP = new EkiOCPP(OCPPsocket.Connect($"ws://0.0.0.0:{Config.Port}"));
            }

            return ekiOCPP.ocppSocket;
        }
        
        public static ChargePoint getCP(string serial)
        {
            //Log.print($"EkiOCPP getCP  socket instance->{OCPPsocket.Instance}");
            var cp = OCPPsocket.Instance?.getCP(serial);
            //Log.print($"EkiOCPP cp->{cp?.serial}");
            return cp;
        }

 

        /// <summary>
        /// 這是因為stop的時候 底下無CP會導致cache會無法清除
        /// </summary>
        /// <param name="serial"></param>
        /// <param name="isAsync"></param>
        public static void stopTranscation(string serial,bool isAsync=true)
        {
            var cp = getCP(serial);
            //空值也送 是為了紀錄cache 當CP重新連上線的時候 再送一次
            var send = cp == null ? true : cp.status == OCPP_Status.CP.Charging;

            if(send)
                if (isAsync)
                    sendCallAsync(serial, new RemoteStopTransactionCall());
                else
                    sendCall(serial, new RemoteStopTransactionCall());




            //getCP(serial).notNull(cp =>
            //{
            //    if (cp.status != OCPP_Status.CP.Charging)
            //    {
            //        if (isAsync)
            //            sendCallAsync(serial, new RemoteStopTransactionCall());
            //        else
            //            sendCall(serial, new RemoteStopTransactionCall());
            //    }
            //});
        }
        //public static void startTranscation(string serial, RemoteStartTransactionCall call)
        //{
        //    ekiOCPP.sendAsync(serial, call);
        //}
        //public static void startReservaNow(string serial, ReservaNowCall call)
        //{
        //    ekiOCPP.sendAsync(serial, call);
        //}

        public static void getCpSchedule(string serial, GetCompositeScheduleCall req = null)
        {
            if (req == null)
            {
                ekiOCPP.sendAsync(serial, new GetCompositeScheduleCall()
                {
                    connectorId = 0,
                    duration = TimeSpan.FromHours(24).TotalSeconds.toInt()
                });
            }
            else
            {
                ekiOCPP.sendAsync(serial, req);
            }
        }

        public static void getCpConfig(string serial)
        {
            ekiOCPP.sendAsync(serial, new GetConfigurationCall());
        }
        /*
         這好像暫時沒用
         */
        public static void getCpLocListVer(string serial)
        {
            ekiOCPP.sendAsync(serial, new GetLocalListVersionCall());
        }

        public static Task<bool> sendCallAsync<T>(string serial, IOCPP_SendPayload<T> payload) where T : IOCPP_Payload
            => ekiOCPP.sendAsync(serial, payload);

        public static bool sendCall<T>(string serial, IOCPP_SendPayload<T> payload) where T : IOCPP_Payload
            => ekiOCPP.send(serial, payload);

        public static bool hasSendCache(string cpSerial = null)
            => sendCache.Where(c => cpSerial != null ? c.cpSerial == cpSerial : true).Count() > 0;

        /// <summary>
        /// 目的在於 預防指令發出的時候 充電樁沒接上線
        /// 等上線的時候在heartbeat的時候觸發檢查
        /// </summary>
        /// <param name="cpSerial"></param>
        /// <returns></returns>
        public static bool executeCache(string cpSerial=null)
        {
            try
            {
                var cacheList = (from c in sendCache
                                 where cpSerial != null ? c.cpSerial == cpSerial : true //null=全執行
                                 select c).toSafeList();

                cacheList.ForEach(cache =>
                {
                    var call = cache.sort.getCall();
                    switch (call.getAction())
                    {
                        case OCPP_Action.RemoteStopTransaction:
                            getCP(cpSerial).notNull(cp =>
                            {
                                if (cp.status == OCPP_Status.CP.Charging)
                                    ekiOCPP.sendAsync(cpSerial, call);
                                else
                                    sendCache.Remove(cache);
                            });                           

                            break;
                        default:
                            ekiOCPP.sendAsync(cpSerial, call);
                            break;
                    }                   
                    
                });

                return true;
            }
            catch(Exception e)
            {
                Log.e("executeCache Error", e);
            }
            return false;
        }

        #endregion



        public Task<bool> sendAsync<T>(string serial, IOCPP_SendPayload<T> payload) where T : IOCPP_Payload
        {
            return Task.Run(() => send(serial, payload));
        }
        public Task<bool> sendAsync(string serial, OCPP_Msg.Call msg)
        {
            return Task.Run(() => send(serial, msg));
        }
        public bool send<T>(string serial, IOCPP_SendPayload<T> payload) where T : IOCPP_Payload
        {           

            var msg = OCPP_Msg.Call.creatByPayload(payload);

            return send(serial, msg);
        }
        public bool send(string serial, OCPP_Msg.Call msg)
        {
            try
            {

                sendCache.creatResultBack(serial, msg);

                var cp = ocppSocket.getCP(serial);
                //Log.print($"send cp->{cp}  total size->{OCPPsocket.cpList}");
                if (cp == null)
                    return false;

                Log.d($"EkiOCPP  send data->{msg.toJsonString()}   cp->{cp}");                

                cp.socket.SendOCPP(msg);
                return true;
            }
            catch (Exception e)
            {
                Log.d($"EkiOCPP send error->{e}");
            }
            return false;
        }

        public void OnOCPP_Open(IWebSocketConnection socket)
        {

            
            //var hasCP = OCPP_CP.checkCP(serial);
            var hasCP = ocppSocket.hasCP(socket);
            //Log.print($"EkiOCPP  onOpen hasCP->{hasCP}   ip->{socket.ConnectionInfo.ClientIpAddress}");
            if (!hasCP)
            {
                ocppSocket.addCP(new ChargePoint(socket));
                //因為有些充電樁重新接上線不會觸發 所以要強制觸發一次 讓boot info能設定上去
                socket.SendOCPP(OCPP_Msg.Call.creatByPayload(TriggerMessageCall.Boot));
                socket.SendOCPP(OCPP_Msg.Call.creatByPayload(TriggerMessageCall.Status));
            }
            else
            {
                //這邊以後可以禁止IP連線 

            }
        }

        public void OnOCPP_Call(IWebSocketConnection socket, OCPP_Msg.Call call)
        {
            try
            {
                var cp = ocppSocket.getCP(socket);
                if (cp == null)
                    return;
                //發送到各個實體裡面 再分別處理
                callSorts.push(call, cp);
            }
            catch(Exception e)
            {
                Log.e($"OnOCPP_Call", e);
            }

        }

        public void OnOCPP_Result(IWebSocketConnection socket, OCPP_Msg.Result result)
        {
            //Log.print($"OnOCPP_Result->{result}  cache->{sendCache.toJsonString()}");

            var cache = sendCache.FirstOrDefault(c => c.sort.msgID() == result.getMsgID());
            if (cache == null)
                return;
            var cp = ocppSocket.getCP(socket);
            if (cp == null)
                return;


            cache.executeResult(result, cp);

            sendCache.Remove(cache);
        }

        private class CpCallSort : List<ICallSort<IOCPP_Payload>>
        {

            public CpCallSort()
            {
                //這改成紀錄型態用的好了
                Add(new AuthorizeNotifySort());
                Add(new BootNotifySort());
                Add(new HeartbeatNotifySort());
                Add(new StatusNotifySort());
                Add(new StartTransactionSort());
                Add(new StopTransactionSort());
                Add(new MeterValuesSort());
            }

            public void push(OCPP_Msg.Call call, ChargePoint cp)
            {
                var action = call.getAction();
                var sort = this.FirstOrDefault(p => p.callAction() == action);

                if (sort == null)
                    return;

                //sort.callPayload(call);
                //sort.onCall(call, cp);
                //Log.print($"EkiOCPP Thread->{System.Threading.Thread.CurrentThread.ManagedThreadId}");

                //建立一個新實體 然後  各自獨立運行
                sort = Activator.CreateInstance(sort.GetType()) as ICallSort<IOCPP_Payload>;

                Task.Run(() =>
                {
                    sort.callPayload(call);
                    sort.onCall(call, cp);
                });

            }
        }

        private class SendMsgCache : List<SendCache>
        {
            //這邊只是記錄型態用
            private List<IResultSort<IOCPP_Payload>> backTypeList = new List<IResultSort<IOCPP_Payload>>()
            {
                new CompositeScheduleResultSort(""),
                new ReservaNowResultSort(""),
                new RemoteStartTransactionResultSort(""),
                new RemoteStopTransactionResultSort(""),
                new SendLocalListResultSort(""),
                new TriggerMessageResultSort(""),
                new ClearCacheResultSort(""),
                new UnlockConnectorResultSort("")
            };
            public void creatResultBack(string cpSerial ,OCPP_Msg.Call call)
            {
                var type = backTypeList.FirstOrDefault(t => t.mapCallAction() == call.getAction())?.GetType();
                if (type == null)
                    return;
                //避免重複
                if (!new ValidCacheRule(cpSerial, call).isInRule(this))
                    return;

                var back = (IResultSort<IOCPP_Payload>)Activator.CreateInstance(type, call.getMsgID());
                back.setCall(call);
                Add(new SendCache(cpSerial, back));
            }
        }

        public class SendCache
        {
            public string cpSerial;
            public IResultSort<IOCPP_Payload> sort;
            public SendCache(string serial, IResultSort<IOCPP_Payload> sort)
            {
                cpSerial = serial;
                this.sort = sort;
            }

            public void executeResult(OCPP_Msg.Result result, ChargePoint cp)
            {
                Task.Run(() =>
                {
                    sort.resultPayload(result);
                    sort.onCallResult(result, cp);
                });
            }
        }

        //有效的=true 
        private class ValidCacheRule : IRuleCheck<SendMsgCache>
        {
            private OCPP_Msg.Call otherCall;
            private string cpSerial;
            public ValidCacheRule(string s,OCPP_Msg.Call other)
            {
                cpSerial = s;
                otherCall = other;
            }
            public bool isInRule(SendMsgCache factor)
            {
                //已經有的也不用再放入
                if (factor.Any(c => c.sort.getCall().getMsgID() == otherCall.getMsgID()))
                    return false;

                //同類型的Action
                if (factor.Where(c => c.cpSerial == cpSerial && c.sort.getCall().getAction() == otherCall.getAction()).Count() >= Config.AllowCacheAction)
                    return false;

                return true;
            }
        }

        #region Timer
        private class CleanErrorCpTimer : Timer,IRunable
        {
            private int counter = 0;
            public CleanErrorCpTimer() : base(Config.CleanSocketInterval)
            {
                AutoReset = true;
                //Enabled = true;
                Elapsed += new ElapsedEventHandler(clean);
            }

            private void clean(object sender, ElapsedEventArgs e)
            {
                try
                {
                    var errorCP = (from s in cpList
                                   where s.serial.isNullOrEmpty()

                                   select s).toSafeList();
                    //Log.print($"clean error cp size->{errorCP.Count()}");
                    errorCP.ForEach(cp =>
                    {
                        //Log.print($"remove cp->{cp}");
                        cp.remove();
                    });

                    //Log.print($"after clean cp count->{cpList.Count}");
                }
                catch(Exception ex)
                {
                    Log.e("Clean Error", ex);
                }

            }

            public void run()
            {
                Start();
            }
        }
        #endregion

        #region Config
        public class Config
        {
            //測試站
            public const string Port = "3001";
            //正式站
            //public const string Port = "5001";

            public const int HeartbeatInterval = 60;//間隔60秒
            //public const int CallStatusIntervalNum = 5;//每間隔5次heartbeat取得一次CP狀態
            public const int OCPP_Version = 1;
            public const int AllowCacheAction = 1;//目前同一台CP只允許暫存同類型的action 1個
            public const int CleanSocketInterval = 1000 * 60 * 5;//5分鐘清除一次
            public const string EkiAdminIdTag = "EkiAdmin";

            public static LocalAuthorization AdminAuth = LocalAuthorization.creatAccepted(EkiAdminIdTag);
        }
        #endregion
    }
}

