using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SendLocalListResult 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class SendLocalListResult:IOCPP_Payload,IEnumConvert
    {
        [StatusEnum(typeof(OCPP_Status.SendLocalList))]
        public string status { get; set; }//OCPP_Status.SendLocalList

    }
}
