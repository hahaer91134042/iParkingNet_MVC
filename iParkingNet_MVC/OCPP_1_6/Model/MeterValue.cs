using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// MeterValue 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    /// <summary>
    /// 通常會有一筆 沒有就回傳錯誤
    /// </summary>
    public class MeterValue:List<MeterValue.Item>
    {
        

        public class Item:IOCPP_Time
        {
            public string timestamp { get; set; }
            public SampleValue sampledValue { get; set; }

            public string timeStr() => timestamp;
        }

    }
}
