using DevLibs;
using OCPP_1_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// AuthorizeNotifySort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class AuthorizeNotifySort : BaseCallMsgSort<AuthorizeCall>
    {
        public override OCPP_Action callAction() => OCPP_Action.Authorize;

        public override void onCall(OCPP_Msg.Call call, ChargePoint cp)
        {
            Log.d($"AuthorizeNotify  onCall auth->{payload.toJsonString()}");
            var authResult = call.callToResult();

            cp.auth = checkCpValid(cp.serial, payload.idTag);
            cp.idTag = cp.auth == OCPP_Status.Authorize.Accepted ? payload.idTag : "";

            authResult.setPayload(new AuthorizeResult().Also(r => r.status = cp.auth));
            Log.d($"AuthorizeNotify result data->{authResult.toJsonString()}");
            cp.socket.SendOCPP(authResult);
            
        }
    }
}