using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// GetLocalListVersionReq 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class GetLocalListVersionCall : IOCPP_Payload, IOCPP_SendPayload<GetLocalListVersionCall>
    {
        public OCPP_Action ocppAction() => OCPP_Action.GetLocalListVersion;

        public GetLocalListVersionCall ocppPayload() => this;
    }
}
