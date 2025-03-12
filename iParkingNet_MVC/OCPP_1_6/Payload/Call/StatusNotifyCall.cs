using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// StatusNotifyReq 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class StatusNotifyCall:IOCPP_Payload,IEnumConvert
    {
        public int connectorId { get; set; }
        public string errorCode { get; set; }
        
        [StatusEnum(typeof(OCPP_Status.CP))]
        public string status { get; set; }

    }
}
