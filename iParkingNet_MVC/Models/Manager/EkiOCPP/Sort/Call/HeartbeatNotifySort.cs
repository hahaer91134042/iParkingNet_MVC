using DevLibs;
using OCPP_1_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// HeartbeatNotifySort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class HeartbeatNotifySort : BaseCallMsgSort<IOCPP_Payload>
    {
        public override OCPP_Action callAction() => OCPP_Action.Heartbeat;

        //private int count = 0;
        public override void onCall(OCPP_Msg.Call call, ChargePoint cp)
        {
            var beatResult = call.callToResult();
            beatResult.setPayload(new HeartbeatResult());
            Log.d($"{GetType().Name} result data->{beatResult.toJsonString()}");
            cp.socket.SendOCPP(beatResult);

            Log.d($"Cache Num->{EkiOCPP.cacheNum}  cp status->{cp.status}");

            //定時去清cache
            EkiOCPP.executeCache(cp.serial);

            
            //以後再考慮好了 需要再開啟
            //if (count % EkiOCPPconfig.CallStatusIntervalNum == 0)
            //{
            //    EkiOCPP.sendCallAsync(cp.serial, TriggerMessageCall.Status);
            //    count = 0;
            //}

            //count++;
            
        }
    }
}