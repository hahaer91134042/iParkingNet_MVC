using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// LocalAuthorization 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class LocalAuthorization
    {
        public static LocalAuthorization creatAccepted(string tag)
            => new LocalAuthorization
            {
                idTag = tag
            }.Also(l =>
            {
                l.idTagInfo.status = OCPP_Status.Authorize.Accepted.ToString();
            });

        public string idTag { get; set; }
        public IdTagInfo idTagInfo { get; set; } = new IdTagInfo();
    }
}
