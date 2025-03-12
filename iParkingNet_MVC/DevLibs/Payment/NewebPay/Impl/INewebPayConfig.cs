using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// INewWebPayCrypto 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public interface INewebPayConfig
    {
        string url();
        string merchantID();
        string hashKey();
        string hashIV();
    }
}
