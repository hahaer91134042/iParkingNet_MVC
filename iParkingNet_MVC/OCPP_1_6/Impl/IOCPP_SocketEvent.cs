using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fleck;

/// <summary>
/// ICOPP_SocketEvent 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public interface IOCPP_SocketEvent
    {
        void OnOCPP_Open(IWebSocketConnection socket);
        void OnOCPP_Call(IWebSocketConnection socket, OCPP_Msg.Call call);
        void OnOCPP_Result(IWebSocketConnection socket, OCPP_Msg.Result result);
    }
}
