using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;
using OCPP_1_6;

/// <summary>
/// ClearCacheResultSort 的摘要描述
/// </summary>
namespace Eki_OCPP
{
    public class ClearCacheResultSort : BaseCallResultSort<ClearCacheResult>
    {
        public ClearCacheResultSort(string mId) : base(mId)
        {
        }

        public override OCPP_Action mapCallAction() => OCPP_Action.ClearCache;

        public override void onCallResult(OCPP_Msg.Result result, ChargePoint cp)
        {
            //Log.print($"{GetType().Name} onCallResult->{result.toJsonString()}");
        }
    }
}
