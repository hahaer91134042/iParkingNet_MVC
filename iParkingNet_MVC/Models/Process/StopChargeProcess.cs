using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Eki_OCPP;
using OCPP_1_6;

/// <summary>
/// StopCpProcess 的摘要描述
/// </summary>
public class StopChargeProcess : BaseProcess
{
    private OCPP_CP cp;
    public StopChargeProcess(EkiOrder order):this(new Location().Also(l=>l.CreatById(order.LocationId)))
    {

    }
    public StopChargeProcess(Location loc)
    {
        cp = loc.Cp;
    }

    public override void run()
    {
        if (cp == null)
            return;

        //EkiOCPP.stopTranscation(cp);

        EkiOCPP.stopTranscation(cp.CpSerial, false);
        //EkiOCPP.sendCall(cp.CpSerial, new RemoteStopTransactionCall());



        //考慮到CP可能斷線的因素 所以改到Hearbeat來執行指令 這邊只call起來而已
        //EkiOCPP.sendCall(cp.CpSerial, TriggerMessageCall.Heartbeat);
    }
}