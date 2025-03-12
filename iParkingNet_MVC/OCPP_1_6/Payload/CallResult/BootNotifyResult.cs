using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BootNotifyResp 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class BootNotifyResult : IOCPP_Payload,IEnumConvert
    {
        [StatusEnum(typeof(OCPP_Status.Boot))]
        public string status{ get;set; }
        public string currentTime = OCPP_Util.nowTime();

        /**
         表示下次送Boot的間隔時間(秒)
         */
        public int interval = OCPP_Config.BootInterval;


    }
}
