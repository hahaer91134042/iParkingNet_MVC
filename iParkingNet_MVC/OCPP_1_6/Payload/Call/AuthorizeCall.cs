using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// AuthorizeCall 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class AuthorizeCall:IOCPP_Payload
    {
        public string idTag { get; set; }
    }
}
