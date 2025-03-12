using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// RemoteStopTransactionCall 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class RemoteStopTransactionCall : IOCPP_Payload, IOCPP_SendPayload<RemoteStopTransactionCall>
    {

        public int transactionId { get; set; } = 1;

        public OCPP_Action ocppAction() => OCPP_Action.RemoteStopTransaction;

        public RemoteStopTransactionCall ocppPayload() => this;
    }
}
