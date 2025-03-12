using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// TriggerMessageCall 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class TriggerMessageCall:IOCPP_Payload,IOCPP_SendPayload<TriggerMessageCall>
    {
        public static TriggerMessageCall Boot = new TriggerMessageCall
        {
            requestedMessage = Action.BootNotification
        };
        public static TriggerMessageCall Status = new TriggerMessageCall
        {
            requestedMessage = Action.StatusNotification
        };
        public static TriggerMessageCall Meter = new TriggerMessageCall
        {
            requestedMessage = Action.MeterValues
        };
        public static TriggerMessageCall Heartbeat = new TriggerMessageCall
        {
            requestedMessage = Action.Heartbeat
        };
        public static TriggerMessageCall Firmware = new TriggerMessageCall
        {
            requestedMessage = Action.FirmwareStatusNotification
        };
        public static TriggerMessageCall Diagnostic = new TriggerMessageCall
        {
            requestedMessage=Action.DiagnosticsStatusNotification
        };

        public string requestedMessage { get; set; }//使用TriggerMessageCall.Action
        public int connectorId { get; set; } = 1;

        public class Action
        {
            public const string BootNotification = "BootNotification";
            public const string DiagnosticsStatusNotification = "DiagnosticsStatusNotification";
            public const string FirmwareStatusNotification = "FirmwareStatusNotification";
            public const string Heartbeat = "Heartbeat";
            public const string MeterValues = "MeterValues";
            public const string StatusNotification = "StatusNotification";
        }

        public OCPP_Action ocppAction() => OCPP_Action.TriggerMessage;

        public TriggerMessageCall ocppPayload() => this;
    }
}
