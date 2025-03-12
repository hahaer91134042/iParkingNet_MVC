using DevLibs;
using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OCPP_1_6;

/// <summary>
/// OCPPsocket 的摘要描述
/// </summary>
public class OCPPsocket : FleckSocket<OCPP_Msg>
{
    protected override string[] subProtocols => new string[] { OCPP_Config.ProtocolVer };

    public static OCPPsocket Instance;
    public static List<ChargePoint> cpList = new List<ChargePoint>();

    //public List<IOCPP_SocketEvent> socketEvents = new List<IOCPP_SocketEvent>();



    public Action<IWebSocketConnection> openEvent;
    public Action<IWebSocketConnection, OCPP_Msg.Call> callEvent;
    public Action<IWebSocketConnection, OCPP_Msg.Result> resultEvent;

    public static OCPPsocket Connect(string url)
    {
        if (Instance == null || Instance?.url != url)
            Instance = new OCPPsocket(url);

        return Instance;
    }

    public OCPPsocket(string url) : base(url)
    {
    }

    protected override void OnSocketBinary(IWebSocketConnection socket, byte[] byteArray)
    {

    }

    protected override void OnSocketClose(IWebSocketConnection socket)
    {
        //Log.print($"ocpp socket on close path->{socket.ConnectionInfo.Path}");
        //Log.print($"ocpp socket onClose instance->{socket.GetHashCode()}");
        removeCP(socket);
    }

    protected override void OnSocketMessage(IWebSocketConnection socket, OCPP_Msg msg)
    {
        //Log.print($"ocpp socket on message->{msg.toJsonString()} msgType->{msg.getMsgType()}");

        //Log.print($"ocpp socket onMessage instance->{socket.GetHashCode()}");
        //socket.Send($"ocpp socket on message->{message.toJsonString()}");
        if (hasCP(socket))
            switch (msg.getMsgType())
            {
                case MsgType.Call:
                    var call = msg.msgToCall();
                    //socketEvents.ForEach(e => e.OnOCPP_Call(socket, call));
                    callEvent(socket, call);
                    //initCall(socket, call);
                    break;
                case MsgType.CallResult:
                    //socketEvents.ForEach(e => e.OnOCPP_Result(socket, msg.msgToResult()));
                    resultEvent(socket, msg.msgToResult());
                    //initResult(socket, msg.toResult());
                    break;
            }
    }

    protected override void OnSocketOpen(IWebSocketConnection socket)
    {
        Log.d($"ocpp socket on open info path->{socket.ConnectionInfo.Path}");
        //Log.print($"ocpp socket onOpen  instance->{socket.GetHashCode()}");

        //目前只有序號
        //var serial = socket.ConnectionInfo.Path.Substring(1);
        //Log.print($"ocpp onOpen serial->{serial}");

        //var oldCp = (from c in cpList
        //             where c.serial == serial
        //             select c).toSafeList();

        //Log.print($"old cp size->{oldCp.Count}");

        openEvent(socket);

        //socketEvents.ForEach(e=>e.OnOCPP_Open(socket, serial));

        //addCP(new ChargePoint(socket,new BootNotifyCall { chargePointSerialNumber=serial}));

    }

    public void addEvent(IOCPP_SocketEvent e)
    {
        //Log.print($"OCPPsocket addEvent {e}");
        //socketEvents.Add(e);
        openEvent += e.OnOCPP_Open;
        callEvent += e.OnOCPP_Call;
        resultEvent += e.OnOCPP_Result;
    }


    //public void removeEvent(IOCPP_SocketEvent e) => socketEvents.Remove(e);
    public void removeEvent(IOCPP_SocketEvent e)
    {
        openEvent -= e.OnOCPP_Open;
        callEvent -= e.OnOCPP_Call;
        resultEvent -= e.OnOCPP_Result;
    }

    public ChargePoint getCP(string serial) => cpList.FirstOrDefault(c => c.info.chargePointSerialNumber.Equals(serial, StringComparison.Ordinal));
    public ChargePoint getCP(IWebSocketConnection socket) => cpList.FirstOrDefault(c => c.socket == socket);
    public string getCpSerial(IWebSocketConnection socket)
    {
        var cp = getCP(socket);
        return cp == null ? "" : cp?.info?.chargePointSerialNumber;
    }

    public bool hasCP(IWebSocketConnection socket) => cpList.Any(c => c.socket == socket);
    public bool hasCP(string serial) => cpList.Any(c => c.info.chargePointSerialNumber.Equals(serial, StringComparison.Ordinal));
    public void removeCP(ChargePoint cp)
    {
        if (cpList.Contains(cp))
            cpList.Remove(cp);
    }
    public void removeCP(IWebSocketConnection socket)
    {
        var cp = cpList.FirstOrDefault(c => c.socket == socket);
        if (cp != null)
        {
            cpList.Remove(cp);
        }
    }

    public void addCP(ChargePoint newCp)
    {
        //var cp = cpList.FirstOrDefault(c => c.info.chargePointSerialNumber.Equals(newCp.info.chargePointSerialNumber, StringComparison.Ordinal));
        var cp = cpList.FirstOrDefault(c => c.socket == newCp.socket);

        //Log.print($"newCp socket->{newCp?.socket?.GetHashCode()} oldCp socket->{cp?.socket?.GetHashCode()}");
        //Log.print($"newCp socket serial->{newCp?.serial} oldCp socket->{cp?.serial}");
        if (cp == null)
        {
            cpList.Add(newCp);
        }
        //else
        //{
        //    cp.refresh(newCp);
        //}
    }

    //private void initResult(IWebSocketConnection socket,OCPP_Msg.Result result)
    //{

    //}

    //private void initCall(IWebSocketConnection socket ,OCPP_Msg.Call req)
    //{
    //    switch (req.getAction())
    //    {
    //        case OCPP_Action.BootNotification:
    //            var client = req.getPayload<BootNotifyCall>();
    //            Log.print($"ocpp socket Boot client->{client.toJsonString()}");

    //            /*
    //             之後這邊做裝置認證的步驟
    //             */
    //            addCP(new ChargePoint(socket, client));



    //            var bootResult = req.creatResult();
    //            bootResult.setPayload(new BootNotifyResult { bootStatus = OCPP_Status.Boot.Accepted });

    //            Log.print($"ocpp socket Boot response data->{bootResult.toJsonString()}");

    //            socket.SendOCPP(bootResult);

    //            break;

    //        case OCPP_Action.StatusNotification:
    //            var statusResult = req.creatResult();                
    //            statusResult.setPayload(new StatusNotifyResult());
    //            Log.print($"ocpp socket StatusNotify response data->{statusResult.toJsonString()}");
    //            socket.SendOCPP(statusResult);
    //            break;
    //        case OCPP_Action.Heartbeat:
    //            var beatResult = req.creatResult();
    //            beatResult.setPayload(new HeartbeatResult());
    //            Log.print($"ocpp socket Heartbeat response data->{beatResult.toJsonString()}");
    //            socket.SendOCPP(beatResult);
    //            break;
    //    }
    //}


}