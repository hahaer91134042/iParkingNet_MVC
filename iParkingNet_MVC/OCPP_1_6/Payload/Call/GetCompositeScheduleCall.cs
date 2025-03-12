using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// GetCompositeScheduleReq 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class GetCompositeScheduleCall : IOCPP_Payload,IOCPP_SendPayload<GetCompositeScheduleCall>
    {

        public int connectorId { get; set; }

        //單位(秒)
        public int duration { get; set; }


        public OCPP_Action ocppAction() => OCPP_Action.GetCompositeSchedule;

        public GetCompositeScheduleCall ocppPayload() => this;

    }
}
