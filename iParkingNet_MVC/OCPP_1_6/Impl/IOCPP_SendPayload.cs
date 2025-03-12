using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OCPP_Ctrl 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public interface IOCPP_SendPayload<R> where R:IOCPP_Payload
    {
        OCPP_Action ocppAction();
        R ocppPayload();
        
    }
}
