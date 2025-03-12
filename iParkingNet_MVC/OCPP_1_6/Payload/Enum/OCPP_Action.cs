using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OCPP_Action 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public enum OCPP_Action
    {        

        Authorize,
        BootNotification,
        StatusNotification,
        Heartbeat,
        MeterValues,

        StartTransaction,
        StopTransaction,

        GetCompositeSchedule,
        GetConfiguration,
        GetLocalListVersion,
        ReserveNow,
        RemoteStartTransaction,
        RemoteStopTransaction,
        SendLocalList,
        TriggerMessage,
        ClearCache,
        UnlockConnector

    }
}
