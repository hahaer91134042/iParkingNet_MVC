using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OCPP_ErrorCode 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class OCPP_ErrorCode
    {
        /*
         * Failure to lock or unlock connector.
         */
        public const string ConnectorLockFailure = "ConnectorLockFailure";

        /*
         * Communication failure with the vehicle, might be Mode 3 or other communication protocol problem. This is
         * not a real error in the sense that the Charge Point doesn’t need to go to the faulted state. Instead, it should go
         * to the SuspendedEVSE state.
         */
        public const string EVCommunicationError = "EVCommunicationError";

        /*
         *Ground fault circuit interrupter has been activated.
         */
        public const string GroundFailure = "GroundFailure";

        /*
         * Temperature inside Charge Point is too high.
         */
        public const string HighTemperature = "HighTemperature";

        /*
         * Error in internal hard- or software component.         
         */
        public const string InternalError = "InternalError";

        /*
         * The authorization information received from the Central System is in conflict with the LocalAuthorizationList.
         */
        public const string LocalListConflict = "LocalListConflict";

        /*
         No error to report.
         */
        public const string NoError = "NoError";

        /*
         Other type of error. More information in vendorErrorCode.
         */
        public const string OtherError = "OtherError";

        /*
         Over current protection device has tripped.
         */
        public const string OverCurrentFailure = "OverCurrentFailure";

        /*
         Voltage has risen above an acceptable level.
         */
        public const string OverVoltage = "OverVoltage";

        /*
         Failure to read electrical/energy/power meter.
         */
        public const string PowerMeterFailure = "PowerMeterFailure";

        /*
         Failure to control power switch.
         */
        public const string PowerSwitchFailure = "PowerSwitchFailure";

        /*
         Failure with idTag reader.
         */
        public const string ReaderFailure = "ReaderFailure";

        /*
         Unable to perform a reset.
         */
        public const string ResetFailure = "ResetFailure";

        /*
         Voltage has dropped below an acceptable level.
         */
        public const string UnderVoltage = "UnderVoltage";

        /*
         Wireless communication device reports a weak signal.
         */
        public const string WeakSignal = "WeakSignal";
    }
}
