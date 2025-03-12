using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Eki_OCPP;
using OCPP_1_6;

/// <summary>
/// StartChargeProcss 的摘要描述
/// </summary>
public class StartChargeProcess:BaseProcess
{
    private OCPP_CP cp;
    public StartChargeProcess(EkiOrder order) : this(new Location().Also(l => l.CreatById(order.LocationId)))
    {

    }
    public StartChargeProcess(Location loc)
    {
        cp = loc.Cp;
    }

    public override void run()
    {
        if (cp == null)
            return;

        EkiOCPP.sendCall(cp.CpSerial, new EkiRemoStart());
    }
}