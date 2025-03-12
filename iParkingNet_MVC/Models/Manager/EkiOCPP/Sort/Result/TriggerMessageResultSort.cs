using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OCPP_1_6;

/// <summary>
/// TriggerMessageResultSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class TriggerMessageResultSort : BaseCallResultSort<TriggerMessageResult>
    {
        public TriggerMessageResultSort(string mId) : base(mId)
        {
        }

        public override OCPP_Action mapCallAction() => OCPP_Action.TriggerMessage;

        public override void onCallResult(OCPP_Msg.Result result, ChargePoint cp)
        {
            
        }
    }
}
