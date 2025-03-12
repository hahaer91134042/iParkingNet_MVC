using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// RemoteStartTransactionResult 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class RemoteStartTransactionResult:IOCPP_Payload,IEnumConvert
    {
        [StatusEnum(typeof(OCPP_Status.Transaction))]
        public string status { get; set; }
    }
}
