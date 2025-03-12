using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// TransactionData 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class TransactionData:List<TransactionData.Item>
    {        

        public class Item:IOCPP_Time
        {
            public string timestamp { get; set; }
            public SampleValue sampledValue { get; set; }

            public string timeStr() => timestamp;
        }


    }
}
