using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;
using OCPP_1_6;

/// <summary>
/// BaseCallMsgSort 的摘要描述
/// 主要是處理CP主動call serve的msg
/// </summary>
namespace Eki_OCPP
{
    public abstract class BaseCallMsgSort<T> : ICallSort<T> where T : IOCPP_Payload
    {
        public T payload;
        public abstract OCPP_Action callAction();
        public void callPayload(OCPP_Msg.Call call) => payload = call.getPayload<T>();
        public abstract void onCall(OCPP_Msg.Call call, ChargePoint cp);

        protected OCPP_Status.Authorize checkCpValid(string cpSerial, string auth)
        {
            //飛宏會送來判斷
            if (auth == EkiOCPP.Config.EkiAdminIdTag)
                return OCPP_Status.Authorize.Accepted;

            return OCPP_Auth.checkAuth(cpSerial, auth) ? OCPP_Status.Authorize.Accepted : OCPP_Status.Authorize.Invalid;
        }
            

    }
}