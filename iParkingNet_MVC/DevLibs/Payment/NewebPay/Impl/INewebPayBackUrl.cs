using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// INewebPayBackUrl 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public interface INewebPayBackUrl
    {
        string returnUrl(params string[] args);
        string notifyUrl(params string[] args);
    }
}
