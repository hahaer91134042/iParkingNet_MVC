using DevLibs;
using OCPP_1_6;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// RemoteStartTransactionResultSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class RemoteStartTransactionResultSort : BaseCallResultSort<RemoteStartTransactionResult>
    {
        public RemoteStartTransactionResultSort(string mId) : base(mId) { }

        public override OCPP_Action mapCallAction() => OCPP_Action.RemoteStartTransaction;

        public override void onCallResult(OCPP_Msg.Result result, ChargePoint cp)
        {
            Log.d($"{GetType().Name} ocCallResult payload->{payload.toJsonString()}");
            switch (payload.status.toEnum<OCPP_Status.Transaction>())
            {
                case OCPP_Status.Transaction.Accepted:
                    cp.auth = OCPP_Status.Authorize.Accepted;
                    break;
            }
        }
    }
}