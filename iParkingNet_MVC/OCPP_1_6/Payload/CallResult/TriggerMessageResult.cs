using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// TriggerMessageResult 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class TriggerMessageResult:IOCPP_Payload,IEnumConvert
    {
        [StatusEnum(typeof(OCPP_Status.TriggerMsg))]
        public string status { get; set; }//OCPP_Status.TriggerMsg


    }
}
