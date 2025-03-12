using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ClearCacheResult 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class ClearCacheResult:IOCPP_Payload,IEnumConvert
    {
        [StatusEnum(typeof(OCPP_Status.ClearCache))]
        public string status { get; set; }
    }
}
