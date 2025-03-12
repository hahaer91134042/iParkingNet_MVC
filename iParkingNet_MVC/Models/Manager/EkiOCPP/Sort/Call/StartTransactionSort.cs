using DevLibs;
using OCPP_1_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// StartTransactionSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class StartTransactionSort : BaseCallMsgSort<StartTranscationCall>
    {
        public override OCPP_Action callAction() => OCPP_Action.StartTransaction;

        public override void onCall(OCPP_Msg.Call call, ChargePoint cp)
        {
            //Log.print($"StartTranscationNotify  onCall ->{payload.toJsonString()}");

            var result = call.callToResult();
            //if (cp.remoteStart)
            //{
            //    result.setPayload(new StartTranscationResult().Also(r => r.idTagInfo.status = OCPP_Status.Authorize.Accepted));
            //}
            //else
            //{

            //}

            //cp.auth = checkCpValid(cp.serial, payload.idTag);

            //cp.idTag = cp.auth == OCPP_Status.Authorize.Accepted ? payload.idTag : "";

            result.setPayload(new StartTranscationResult().Also(r => r.idTagInfo.setStatus(cp.auth)));

            Log.d($"StartTranscationNotify  result->{result.toJsonString()}");
            cp.socket.SendOCPP(result);
        }
    }
}