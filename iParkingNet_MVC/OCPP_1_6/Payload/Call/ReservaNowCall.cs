using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ReservaNowCall 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class ReservaNowCall : IOCPP_Payload, IOCPP_SendPayload<ReservaNowCall>
    {
        public int connectorId { get; set; } = 1;
        public string expiryDate { get; set; }
        public string idTag { get; set; } = OCPP_Util.creatUid();
        //public string idTag { get; set; } = "F30AF411";
        public int reservationId { get; set; } = 1;

        //public string parentIdTag { get; set; }

        public OCPP_Action ocppAction() => OCPP_Action.ReserveNow;

        public ReservaNowCall ocppPayload() => this;
    }
}
