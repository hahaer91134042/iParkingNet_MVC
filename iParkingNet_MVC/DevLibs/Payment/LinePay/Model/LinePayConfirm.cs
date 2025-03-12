using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LinePayConfirm 的摘要描述
/// </summary>
namespace Eki_LinePayApi_v3
{
    public class LinePayConfirm
    {

        public int amount { get; set; }
        public string currency { get; set; } = LinePay.Currency.TWD.ToString();

    }
}
