using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// HeartbeatResp 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class HeartbeatResult : IOCPP_Payload
    {
        public string currentTime = OCPP_Util.nowTime();
    }
}
