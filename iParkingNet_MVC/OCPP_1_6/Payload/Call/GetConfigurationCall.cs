using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// GetConfiguration 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class GetConfigurationCall : IOCPP_Payload, IOCPP_SendPayload<GetConfigurationCall>
    {
        public OCPP_Action ocppAction() => OCPP_Action.GetConfiguration;

        public GetConfigurationCall ocppPayload() => this;
    }
}
