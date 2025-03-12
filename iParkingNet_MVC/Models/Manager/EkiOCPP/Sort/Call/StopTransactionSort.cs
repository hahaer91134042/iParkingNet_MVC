using DevLibs;
using OCPP_1_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// StopTransactionSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class StopTransactionSort : BaseCallMsgSort<StopTransactionCall>
    {
        public override OCPP_Action callAction() => OCPP_Action.StopTransaction;

        public override void onCall(OCPP_Msg.Call call, ChargePoint cp)
        {
            Log.d($"{GetType().Name} onCall->{payload.toJsonString()}");

            var result = call.callToResult();

            //if (cp.idTag == payload.idTag)
            //{
            //    cp.auth = OCPP_Status.Authorize.Invalid;
            //    cp.idTag = "";
            //}
            //else
            //{
            //    switch (cp.status)
            //    {
            //        case OCPP_Status.CP.Faulted:
            //        case OCPP_Status.CP.Finishing:

            //            cp.auth = OCPP_Status.Authorize.Invalid;
            //            cp.idTag = "";
            //            break;
            //    }
            //}

            //應該可以不用管 這邊只是CP來通知要停止而已
            cp.auth = OCPP_Status.Authorize.Invalid;
            cp.idTag = "";

            //Log.print($"StopTranscation cp auth->{cp.auth}");

            //cp.auth = chechCpValid(cp.serial, payload.idTag);
            //cp.idTag = cp.auth == OCPP_Status.Authorize.Accepted ? payload.idTag : "";

            //這邊要相反
            result.setPayload(new StopTransactionResult().Also(r => r.idTagInfo.setStatus(cp.auth == OCPP_Status.Authorize.Accepted ? OCPP_Status.Authorize.Invalid : OCPP_Status.Authorize.Accepted)));
            Log.d($"StopTranscation  result->{result.toJsonString()}");
            cp.socket.SendOCPP(result);
        }
    }
}