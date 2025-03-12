using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ChargePoint 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class ChargePoint
    {
        public IWebSocketConnection socket;
        public BootNotifyCall info;
        public OCPP_Status.CP status = OCPP_Status.CP.Available;
        public OCPP_Status.Authorize auth = OCPP_Status.Authorize.Invalid;
        public string idTag = "";
        public int connectorId = 1;
        //public bool enable = true;

        public string serial { get => info == null ? "" : info.chargePointSerialNumber; }

        public ChargePoint(IWebSocketConnection s, BootNotifyCall i=null)
        {
            socket = s;
            info = i;
        }

        public void refresh(ChargePoint other)
        {
            socket = other.socket;
            info = other.info;
        }

        public void remove()
        {
            OCPPsocket.Instance.removeCP(this);
        }

        public override string ToString()
        {
            return $"{base.ToString()}#{serial} | socket#{socket.GetHashCode()} | status#{status}";
        }
    }
}