using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OCPP_Config 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class OCPP_Config
    {
        public const string ProtocolVer = "ocpp1.6";
        /*ISO8601*/
        public const string OCPP_TimeFormate = "yyyy-MM-ddThh:mm:ssZ";
        /*OCPP heartbeat 間格時間(秒)*/
        public const int BootInterval = 60;

        public const int UidLength = 20;

        public class FieldPosition
        {
            public const int Type = 0;
            public const int Uid = 1;
            public const int CallAction = 2;
            public const int CallPayload = 3;
            public const int ResultPayload = 2;
        }

    }
}
