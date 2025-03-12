using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// StartTranscationResult 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class StartTranscationResult:IOCPP_Payload
    {
        public IdTagInfo idTagInfo { get; set; } = new IdTagInfo();
        public int transactionId { get; set; } = 1;//這邊基本上隨便
    }
}
