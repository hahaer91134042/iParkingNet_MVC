using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// StopTransactionResult 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class StopTransactionResult:IOCPP_Payload
    {
        public IdTagInfo idTagInfo { get; set; } = new IdTagInfo();
    }
}
