using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// RemoteStartTransactionCall 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class RemoteStartTransactionCall : IOCPP_Payload, IOCPP_SendPayload<RemoteStartTransactionCall>
    {
        public RemoteStartTransactionCall(string tag = null)
        {
            if (tag != null)
                idTag = tag;
        }
        

        public int connectorId { get; set; } = 1;
        public string idTag { get; set; } = OCPP_Util.creatUid();
        //public string idTag { get; set; } = "F30AF411";


        public OCPP_Action ocppAction() => OCPP_Action.RemoteStartTransaction;

        public RemoteStartTransactionCall ocppPayload() => this;
    }
}
