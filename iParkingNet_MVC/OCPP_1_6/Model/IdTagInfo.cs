using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// IdTagInfo 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class IdTagInfo:IEnumConvert
    {
        /*
         Optional. This contains the date at which idTag should be removed from the
         Authorization Cache.
         */
        //public string expiryDate { get; set; }

        /*
        Optional. This contains the parent-identifier.
         */
        //public string parentIdTag { get; set; }
        [StatusEnum(typeof(OCPP_Status.Authorize))]
        public string status { get; set; } = OCPP_Status.Authorize.Invalid.ToString();
    }
}
