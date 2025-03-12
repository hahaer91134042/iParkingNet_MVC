using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// UnlockConnectorCall 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class UnlockConnectorCall : IOCPP_Payload, IOCPP_SendPayload<UnlockConnectorCall>
    {

        public int connectorId { get; set; } = 1;

        public OCPP_Action ocppAction() => OCPP_Action.UnlockConnector;

        public UnlockConnectorCall ocppPayload() => this;
    }
}
