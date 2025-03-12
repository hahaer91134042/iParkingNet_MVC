using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;
using OCPP_1_6;

/// <summary>
/// MeterValuesSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class MeterValuesSort : BaseCallMsgSort<MeterValuesCall>
    {
        public override OCPP_Action callAction() => OCPP_Action.MeterValues;

        public override void onCall(OCPP_Msg.Call call, ChargePoint cp)
        {
            Log.d($"{GetType().Name} onCall->{call.toJsonString()}");

            var result = call.callToResult().Also(r => r.setPayload(new MeterValuesResult()));
            cp.socket.SendOCPP(result);
        }
    }
}
