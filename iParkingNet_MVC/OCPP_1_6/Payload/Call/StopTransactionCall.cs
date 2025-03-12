using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// StopTranscationCall 的摘要描述
/// {"idTag":"F30AF411","meterStop":150,"timestamp":"2022-01-27T09:57:41Z","transactionId":1}
/// </summary>
namespace OCPP_1_6
{
    public class StopTransactionCall:IOCPP_Payload,IOCPP_Time
    {
        public string idTag { get; set; }
        public int meterStop { get; set; }
        public string timestamp { get; set; }
        public int transactionId { get; set; }
        public string reason { get; set; }
        public TransactionData transactionData { get; set; }

        public string timeStr() => timestamp;
    }
}
