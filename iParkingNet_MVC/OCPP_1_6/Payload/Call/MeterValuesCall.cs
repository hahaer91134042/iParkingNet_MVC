using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// MeterValuesCall 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class MeterValuesCall:IOCPP_Payload
    {
        public int connectorId { get; set; }
        public int transactionId { get; set; }
        public MeterValue meterValue { get; set; }

        public SampleValue findSample(int index = 0) => meterValue[index].sampledValue;
    }
}
