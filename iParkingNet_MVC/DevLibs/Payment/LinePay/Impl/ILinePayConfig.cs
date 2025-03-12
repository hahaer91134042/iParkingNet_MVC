using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ILinePayConnect 的摘要描述
/// </summary>
namespace Eki_LinePayApi_v3
{
    public interface ILinePayConfig
    {
        string ChannelID { get; }
        string SecretKey { get; }
        string confirmUrl(params string[] args);
        string cancelUrl(params string[] args);
    }
}
