using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OCPP_1_6;

/// <summary>
/// UnlockConnectorResultSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class UnlockConnectorResultSort : BaseCallResultSort<UnlockConnectorResult>
    {
        public UnlockConnectorResultSort(string mId) : base(mId)
        {
        }

        public override OCPP_Action mapCallAction() => OCPP_Action.UnlockConnector;

        public override void onCallResult(OCPP_Msg.Result result, ChargePoint cp)
        {
            
        }
    }
}
