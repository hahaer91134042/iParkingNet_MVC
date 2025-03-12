using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BootNotify 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class BootNotifyCall:IOCPP_Payload
    {
        public string chargePointVendor { get; set; }
        public string chargePointModel { get; set; }
        public string chargePointSerialNumber { get; set; }
        public string chargeBoxSerialNumber { get; set; }
        public string firmwareVersion { get; set; }
        public string iccid { get; set; } = "";
        public string imsi { get; set; } = "";
        public string meterSerialNumber { get; set; } = "";
        public string meterType { get; set; } = "";
    }
}
