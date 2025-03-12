using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;
using OCPP_1_6;

/// <summary>
/// RemoteStopTransactionResultSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class RemoteStopTransactionResultSort : BaseCallResultSort<RemoteStopTransactionResult>
    {
        public RemoteStopTransactionResultSort(string mId) : base(mId) { }

        public override OCPP_Action mapCallAction() => OCPP_Action.RemoteStopTransaction;

        public override void onCallResult(OCPP_Msg.Result result, ChargePoint cp)
        {
            Log.d($"{GetType().Name} onCallResult payload->{payload.toJsonString()}");
        }
    }
}