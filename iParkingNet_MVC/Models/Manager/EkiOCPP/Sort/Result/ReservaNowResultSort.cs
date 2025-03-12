using DevLibs;
using OCPP_1_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ReservaNowResultSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class ReservaNowResultSort : BaseCallResultSort<ReservaNowResult>
    {
        public ReservaNowResultSort(string mId) : base(mId) { }

        public override OCPP_Action mapCallAction() => OCPP_Action.ReserveNow;

        public override void onCallResult(OCPP_Msg.Result result, ChargePoint cp)
        {
            Log.d($"{GetType().Name} onCallResult payload->{payload.toJsonString()}");
        }
    }
}