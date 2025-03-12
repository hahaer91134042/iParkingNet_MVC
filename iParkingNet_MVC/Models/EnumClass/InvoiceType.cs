using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// InvoiceType 的摘要描述
/// </summary>
public enum InvoiceType
{
    None=0,//不索取
    Donate=1,//捐贈
    Paper =2,//紙本
    Phone=3,//手機載具
    Certificate=4,//自然人憑證
    ezPay=5//ezPay載具
}