using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;
using Fleck;

/// <summary>
/// BroadcastSocket 的摘要描述
/// </summary>
public class BroadcastSocket : FleckSocket<SocketReceiveMsg>
{
    protected static List<SocketUser> allSockets = new List<SocketUser>();

    public static BroadcastSocket Connect(string url)
    {
        if (Instance == null || Instance?.url != url)
            Instance = new BroadcastSocket(url);
        return Instance;
    }
    public static BroadcastSocket Instance = null;

    BroadcastSocket(string url) : base(url)
    {
    }


    protected override void OnSocketBinary(IWebSocketConnection socket, byte[] byteArray)
    {
        
    }

    protected override void OnSocketMessage(IWebSocketConnection socket, SocketReceiveMsg message)
    {

        
       // var user = getSocketUser(socket);

        //message.saveLog(user,"onSocketMessage");


        //socket.Send(new
        //{
        //    Success = true,
        //    Action = "GetMsg",
        //    Msg = message,
        //    Request=message.request.toObj<LoadLocationRequest>()
        //}.toJsonString());

        switch (message.socketMethod)
        {
            case SocketMethod.SendTo:

                break;
            case SocketMethod.SendAll:

                break;
            case SocketMethod.LoadLocation:
                //目前這東西 用起來沒多快 可能還是用普通的web api比較實際
                var request = message.request.toObj<LoadLocationRequest>();
                if (request.isValid())
                {
                    var loadManager = new LoadManager();

                    //var locInRange = request.address.isEmpty() ? loadManager.getLocationFrom(request.lat, request.lng, request.config, request.nowTime) : loadManager.getLocationFrom(request.address, request.config, request.nowTime);
                    var result = loadManager.getLocationFrom(request);
                    //var dataList = (from loc in locInRange.List
                    //                select loc.convertToLoadLocResponse()).toSafeList();

                    socket.Send(result);
                }
                
                break;
        }
    }

    protected override void OnSocketOpen(IWebSocketConnection socket)
    {            
        try
        {

            //Log.print($"PPYP socket on open->");

            var id = getSocketUser(socket);
            //假如有舊的 去掉
            var old = (from user in allSockets
                       where user.UserID == id
                       select user).FirstOrDefault();
            if (!old.isNullOrEmpty())
            {
                old.Socket.Close();
                allSockets.Remove(old);
            }

            allSockets.Add(new SocketUser() { UserID = id, Socket = socket });
        }
        catch (ArgumentNullException)
        {
            allSockets.Add(new SocketUser() { Socket = socket });
        }
        catch (Exception)
        {
        }
        var msg = SocketEventContent.Open.toSocketMsg();
        socket.Send(msg.toJsonString());
    }

    protected override void OnSocketClose(IWebSocketConnection socket)
    {
        var s = (from u in allSockets
                 where u.Socket.ConnectionInfo.Id.ToString() == socket.ConnectionInfo.Id.ToString()
                 select u).FirstOrDefault();
        if (s != null)
        {
            s.Socket.Send(SocketEventContent.Close.toSocketMsg().toJsonString());
            s.Socket.Close();
            allSockets.Remove(s);
        }
    }

    private string getSocketUser(IWebSocketConnection socket)
    {
        //socket.saveLog("add socket");
        //這邊取出ws://host:port/path <-token
        var path = socket.ConnectionInfo.Path.Replace("/", "");
        //path.saveLog("Socket add user token");

        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException();

        var token = HttpUtility.UrlDecode(path);

        var jwtObject = JwtBuilder.GetDecoder()
                            .setToken(token)
                            .decode();

        //if (DateTime.Compare(DateTime.Parse(jwtObject.exp), DateTime.Now) < 0)
        //    throw new ArgumentException();
       

        return jwtObject.user;
    }

    public bool SendTo(string memberUniqueID, ISocketMsg msg) 
        =>SendTo(memberUniqueID, msg.toSocketMsg().toJsonString());
    public bool SendTo(string memberUniqueID, string msg)
    {
        try
        {
            var user = SelectUser(memberUniqueID);
            user.Socket.Send(msg);
            return true;//發送成功
        }
        catch (Exception)
        {
        }
        return false;
    }
    public void SendAll(string msg)
    {
        allSockets.ForEach(user =>
        {
            try
            {
                user?.Socket?.Send(msg);
            }
            catch (Exception)
            {
            }
        });
    }
    public bool ContainUser(string uniqueID)
    {
        return allSockets.Any(user => user.UserID == uniqueID);
        //return (from user in allSockets
        //        where user.UserID == uniqueID
        //        select user).Count() > 0;
    }
    protected SocketUser SelectUser(string uniqueID)
    {
        return allSockets.First(socket => socket.UserID == uniqueID);
        //return (from s in allSockets
        //        where s.UserID == uniqueID
        //        select s).FirstOrDefault();
    }

    protected class SocketUser
    {
        public string UserID;
        public IWebSocketConnection Socket;
    }
    private class SocketResponse : ResponseInfoModel<object>
    {
        public SocketResponse(bool successful) : base(successful)
        {
        }
    }
}