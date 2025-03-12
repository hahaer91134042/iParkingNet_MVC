using DevLibs;
using OCPP_1_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BootNotifySort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class BootNotifySort : BaseCallMsgSort<BootNotifyCall>
    {
        public override OCPP_Action callAction() => OCPP_Action.BootNotification;

        public override void onCall(OCPP_Msg.Call call, ChargePoint cp)
        {
            //Log.print($"BootNotify Thread->{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            Log.d($"BootNotify  client->{payload.toJsonString()}  cp->{cp}");

            /*
             這邊是要更新資料用的
             */
            cp.info = payload;


            var bootResult = call.callToResult();
            bootResult.setPayload(new BootNotifyResult
            {
                status = OCPP_Status.Boot.Accepted.ToString(),
                interval=EkiOCPP.Config.HeartbeatInterval
            });

            //Log.print($"BootNotify result data->{bootResult.toJsonString()}");

            cp.socket.SendOCPP(bootResult);


            //清除一定要放在最後
            //要清除舊的同序號的CP
            (from s in EkiOCPP.cpList
             where s.serial == cp.serial
             where s.socket != cp.socket
             select s).toSafeList().ForEach(old =>
             {
                 old.remove();
             });

            //EkiOCPP.sendCallAsync(cp.serial, new SendLocalListCall(1)
            //{
            //    updateType=UpdateType.Full
            //}.Also(c=> c.localAuthorizationList.Add(EkiOCPP.Config.AdminAuth)));

        }
    }
}