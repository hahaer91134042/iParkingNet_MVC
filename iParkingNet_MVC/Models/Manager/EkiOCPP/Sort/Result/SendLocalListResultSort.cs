using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OCPP_1_6;

/// <summary>
/// SendLocalListResultSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class SendLocalListResultSort : BaseCallResultSort<SendLocalListResult>
    {
        public SendLocalListResultSort(string mId) : base(mId)
        {
        }

        public override OCPP_Action mapCallAction() => OCPP_Action.SendLocalList;

        public override void onCallResult(OCPP_Msg.Result result, ChargePoint cp)
        {
            
        }
    }
}
