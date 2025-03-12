using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ClearCacheCall 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class ClearCacheCall : IOCPP_Payload, IOCPP_SendPayload<ClearCacheCall>
    {
        public OCPP_Action ocppAction() => OCPP_Action.ClearCache;
        public ClearCacheCall ocppPayload() => this;
    }
}
