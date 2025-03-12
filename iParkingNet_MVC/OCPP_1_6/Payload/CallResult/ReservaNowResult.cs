using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ReservaNowResult 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class ReservaNowResult:IOCPP_Payload,IEnumConvert
    {
        [StatusEnum(typeof(OCPP_Status.Reservation))]
        public string status { get; set; }
    }
}
