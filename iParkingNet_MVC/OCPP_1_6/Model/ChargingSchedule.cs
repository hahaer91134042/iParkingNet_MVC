using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ChargingSchedule 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class ChargingSchedule
    {
        public long duration { get; set; }
        public string chargingRateUnit { get; set; }
        public List<Period> chargingSchedulePeriod { get; set; }

        public class Period
        {
            public long startPeriod { get; set; }
            public double limit { get; set; }
            public int numberPhases { get; set; }
        }
    }
}
