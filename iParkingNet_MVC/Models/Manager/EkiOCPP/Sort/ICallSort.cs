using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OCPP_1_6;

/// <summary>
/// ICallSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    //這邊泛型一定要帶out不然 List那邊的泛型會出錯
    public interface ICallSort<out T> where T : IOCPP_Payload
    {
        void callPayload(OCPP_Msg.Call call);
        OCPP_Action callAction();
        void onCall(OCPP_Msg.Call call, ChargePoint cp);
    }
}