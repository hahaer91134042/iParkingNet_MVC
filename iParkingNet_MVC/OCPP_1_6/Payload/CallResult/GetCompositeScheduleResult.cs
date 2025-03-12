using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// GetCompositeScheduleResult 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class GetCompositeScheduleResult:IOCPP_Payload,IOCPP_Time,IEnumConvert
    {
        [StatusEnum(typeof(OCPP_Status.CompositeSchedule))]
        public string status { get; set; }
        public int connectorId { get; set; }
        public string scheduleStart { get; set; }
        public ChargingSchedule chargingSchedule { get; set; }
        public string timeStr() => scheduleStart;
    }
}
