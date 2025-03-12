using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// AuthorizeResult 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class AuthorizeResult:IOCPP_Payload
    {
        public IdTagInfo idTagInfo { get; set; } = new IdTagInfo();

        public OCPP_Status.Authorize status
        {
            get => idTagInfo.status<OCPP_Status.Authorize>();
            set => idTagInfo.setStatus(value);
        }

    }
}
