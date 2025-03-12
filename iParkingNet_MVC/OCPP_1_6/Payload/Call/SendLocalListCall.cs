using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SendLocalListCall 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class SendLocalListCall : IOCPP_Payload, IOCPP_SendPayload<SendLocalListCall>
    {
        public SendLocalListCall(int ver)
        {
            listVersion = ver;
        }
        public int listVersion { get; set; }
        public List<LocalAuthorization> localAuthorizationList { get; set; } = new List<LocalAuthorization>();
        public string updateType { get; set; }

        public OCPP_Action ocppAction() => OCPP_Action.SendLocalList;
        public SendLocalListCall ocppPayload() => this;
    }
}
