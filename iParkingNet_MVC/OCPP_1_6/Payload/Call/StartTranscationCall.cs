using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// StartTranscationCall 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class StartTranscationCall : IOCPP_Payload,IOCPP_Time
    {
        public int connectorId { get; set; }
        public string idTag { get; set; }
        public int meterStart { get; set; }
        public int reservationId { get; set; }
        public string timestamp { get; set; }

        public string timeStr() => timestamp;
    }
}
