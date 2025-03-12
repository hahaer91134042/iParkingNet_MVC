using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OCPP_Status 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class OCPP_Status
    {

        public enum Boot
        {
            Accepted,
            Pending,
            Rejected
        }

        public enum Authorize
        {

             Accepted,

            /*
             Identifier has been blocked. Not allowed for charging.
             */
            Blocked,

            /*
             Identifier has expired. Not allowed for charging.
             */
            Expired,

            /*
             Identifier is unknown. Not allowed for charging.
             */
            Invalid,

            /*
             Identifier is already involved in another transaction and multiple transactions are not allowed. (Only relevant for a
             StartTransaction.req.)
             */
            ConcurrentTx
        }

        /// <summary>
        /// Charge Point status
        /// </summary>
        public enum CP
        {

            /// <summary>
            /// When a Connector becomes available for a new user (Operative)
            /// </summary>
            Available, //基本上是表示待機狀態

            /// <summary>
            /// When a Connector becomes no longer available for a new user but there is no ongoing Transaction (yet). Typically a Connector
            /// is in preparing state when a user presents a tag, inserts a cable or a vehicle occupies the parking bay(Operative)
            /// </summary>
            Preparing,  //準備接收指令

            /// <summary>
            /// When the contactor of a Connector closes, allowing the vehicle to charge(Operative)
            /// </summary>
            Charging,

            /// <summary>
            /// When the EV is connected to the EVSE but the EVSE is not offering energy to the EV, e.g. due to a smart charging restriction,
            ///local supply power constraints, or as the result of StartTransaction.conf indicating that charging is not allowed etc.(Operative)
            /// </summary>
            SuspendedEVSE,

            /// <summary>
            /// When the EV is connected to the EVSE and the EVSE is offering energy but the EV is not taking any energy.(Operative)
            /// </summary>
            SuspendedEV,

            /// <summary>
            /// When a Transaction has stopped at a Connector, but the Connector is not yet available for a new user, e.g. the cable has not
            ///been removed or the vehicle has not left the parking bay(Operative)
            /// </summary>
            Finishing,

            /// <summary>
            /// When a Connector becomes reserved as a result of a Reserve Now command(Operative)
            /// </summary>
            Reserved,

            /// <summary>
            /// When a Connector becomes unavailable as the result of a Change Availability command or an event upon which the Charge
            ///Point transitions to unavailable at its discretion. Upon receipt of a Change Availability command, the status MAY change
            ///immediately or the change MAY be scheduled. When scheduled, the Status Notification shall be send when the availability
            ///change becomes effective(Inoperative)
            /// </summary>
            Unavailable,

            /// <summary>
            /// When a Charge Point or connector has reported an error and is not available for energy delivery . (Inoperative).
            /// </summary>
            Faulted
        }

        /*
        Accepted Reservation has been made.
        Faulted Reservation has not been made, because connectors or specified connector are in a faulted state.
        Occupied Reservation has not been made. All connectors or the specified connector are occupied.
        Rejected Reservation has not been made. Charge Point is not configured to accept reservations.
        Unavailable Reservation has not been made, because connectors or specified connector are in an unavailable state.
         */
        public enum Reservation
        {
            Accepted,
            Faulted,
            Occupied,
            Rejected,
            Unavailable
        }

        /*
         Accepted Request has been accepted and will be executed.
         Rejected Request has not been accepted and will not be executed.
         */
        public enum CompositeSchedule
        {
            Accepted,
            Rejected
        }

        public enum Transaction
        {
            Accepted,
            Rejected
        }

        public enum ClearCache
        {
            Accepted,
            Rejected
        }

        public enum SendLocalList
        {
            Accepted,
            Failed,
            NotSupported,
            VersionMismatch
        }

        public enum Update
        {
            /*Accepted Local Authorization List successfully updated.*/
            Accepted,
            /*Failed Failed to update the Local Authorization List.*/
            Failed,
            /*NotSupported Update of Local Authorization List is not supported by Charge Point.*/
            NotSupported,
            /*VersionMismatch Version number in the request for a differential update is less or equal then version number of current list.*/
            VersionMismatch
        }

        public enum TriggerMsg
        {
            Accepted,
            Rejected,
            NotImplemented
        }

        public enum Unlock
        {

            /// <summary>
            /// Connector has successfully been unlocked.
            /// </summary>
            Unlocked,

            /// <summary>
            /// Failed to unlock the connector: The Charge Point has tried to unlock the connector and has detected that the connector is still
            ///locked or the unlock mechanism failed.
            /// </summary>
            UnlockFailed,

            /// <summary>
            /// Charge Point has no connector lock, or ConnectorId is unknown.
            /// </summary>
            NotSupported
        }
    }
}
