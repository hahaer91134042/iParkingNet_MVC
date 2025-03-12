using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// UnlockConnectorResult 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class UnlockConnectorResult:IOCPP_Payload,IEnumConvert
    {
        [StatusEnum(typeof(OCPP_Status.Unlock))]
        public string status { get; set; }//OCPP_Status.Unlock
    }
}
