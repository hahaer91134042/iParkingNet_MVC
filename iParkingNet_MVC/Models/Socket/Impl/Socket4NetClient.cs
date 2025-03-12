using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSocket4Net;

/// <summary>
/// 使用WebSocket4Net實體的客戶端
/// 目前還沒完成 需要時再改進
/// </summary>
public abstract class Socket4NetClient<MSG> : IClientSocket<WebSocket, MSG>
{
    public Socket4NetClient(string url) : base(url)
    {
    }


    protected override void InitSocket()
    {
        var extension = new List<KeyValuePair<string, string>>();
        extension.Add(new KeyValuePair<string, string>("sec-websocket-extensions", "permessage-deflate; client_max_window_bits, x-webkit-deflate-frame"));
        var client = new WebSocket(url, subProtocol: "", customHeaderItems: extension);
        
        client.Opened += (sender, args) =>
        {
            OnSocketOpen(client);
        };
        client.MessageReceived += (sender, args) =>
        {
            OnSocketMessage(client, args.toObj<MSG>());
        };
        client.DataReceived += (sender, args) =>
        {
            OnSocketBinary(client, args.Data);
        };
        client.Error += (sender, args) =>
        {

        };
        client.Closed += (sender, args) =>
        {
            OnSocketClose(client);
        };

        client.Open();
    }

}