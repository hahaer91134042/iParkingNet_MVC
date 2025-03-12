using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OCPP_1_6;

/// <summary>
/// IResultSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    //這邊泛型一定要帶out不然 List那邊的泛型會出錯
    public interface IResultSort<out T> where T : IOCPP_Payload
    {
        OCPP_Action mapCallAction();
        string msgID();
        void resultPayload(OCPP_Msg.Result result);
        void setCall(OCPP_Msg.Call call);
        OCPP_Msg.Call getCall();

        void onCallResult(OCPP_Msg.Result result, ChargePoint cp);
    }
}