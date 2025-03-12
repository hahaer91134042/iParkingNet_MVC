using DevLibs;
using Fleck;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;

/// <summary>
/// 使用Fleck socket
/// </summary>
public abstract class FleckSocket<RECEIVE>:IServerSocket<IWebSocketConnection,RECEIVE>
{
    protected FleckSocket(string url):base(url)
    {

    }
    protected override  void  InitSocket()
    {

        // **若改成wss://** 則是安全web socket透過X.509認證
        var server = new WebSocketServer(url);
        server.RestartAfterListenError = true;//自動重啟

        server.SupportedSubProtocols = subProtocols;

        server.Start(socket =>
        {
            //var subP = socket.ConnectionInfo.SubProtocol;
            //var nP = socket.ConnectionInfo.NegotiatedSubProtocol;
            //var header = socket.ConnectionInfo.Headers;
            //Log.print("--- socket servar start ---");
            //Log.print($"socket protocol=>{subP}");
            //Log.print($"NegotiatedProtocol->{nP}");
            //Log.print($"socket Header->{header.toJsonString()}");

            // 新的Socket已連線
            socket.OnOpen = () =>
            {


                OnSocketOpen(socket);
            };
            // Socket離線
            socket.OnClose = () =>
            {
                OnSocketClose(socket);
            };
            // 傳送訊息
            socket.OnMessage = message =>
            {
                //Console.WriteLine(message);
                //allSockets.ForEach(s => s.Send("Echo: " + message));
                //var msg = HttpUtility.UrlDecode(message).toObj<SocketReceiveMsg>();

                //Log.print($"socket on message->{message}");

                message.toObj<RECEIVE>().notNull(msg =>
                {
                    OnSocketMessage(socket, msg);
                });                
            };
            socket.OnBinary = byteArray =>
            {
                OnSocketBinary(socket, byteArray);
            };
        });
    }





    //這個解析方式應該是每個繼承的socket去訂製
    //protected string getSocketUser(IWebSocketConnection socket)
    //{
    //    //這邊取出ws://host:port/path <-token
    //    var path = socket.ConnectionInfo.Path.Replace("/", "");
    //    if (string.IsNullOrEmpty(path))
    //        throw new ArgumentNullException();

    //    var token = HttpUtility.UrlDecode(path);

    //    var jwtObject = JwtBuilder.GetDecoder()
    //                        .setToken(token)
    //                        .decode();

    //    if (DateTime.Compare(DateTime.Parse(jwtObject.exp), DateTime.Now) < 0)
    //        throw new ArgumentException();

    //    return jwtObject.user;
    //}
    


    //public static FleckSocket Connect(string url)
    //{
    //    return new FleckSocket(url);
    //}
}