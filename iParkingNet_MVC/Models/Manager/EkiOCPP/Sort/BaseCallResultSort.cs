using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OCPP_1_6;

/// <summary>
/// BaseCallResultSort 的摘要描述
/// 主要是處理Server call出去後 CP回傳的Result
/// </summary>
namespace Eki_OCPP
{
    public abstract class BaseCallResultSort<T> : IResultSort<T> where T : IOCPP_Payload
    {
        public T payload;
        public OCPP_Msg.Call call;
        public string callMsgId;
        public BaseCallResultSort(string mId)
        {
            callMsgId = mId;
        }

        public void resultPayload(OCPP_Msg.Result result) => payload = result.getPayload<T>();


        public abstract void onCallResult(OCPP_Msg.Result result, ChargePoint cp);

        public abstract OCPP_Action mapCallAction();

        public string msgID() => callMsgId;

        public void setCall(OCPP_Msg.Call call)
        {
            this.call = call;
        }

        public OCPP_Msg.Call getCall() => call;
    }
}