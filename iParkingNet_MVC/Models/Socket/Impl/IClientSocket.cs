using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 面向用戶端的socket client(WebSocket4Net)
/// 主要是開啟一個對接服務器的socket
/// </summary>
public abstract class IClientSocket<CLIENT,MSG>
{
    protected string url;
    public IClientSocket(string url)
    {
        this.url = url;
        InitSocket();
    }

    protected abstract void InitSocket();

    protected abstract void OnSocketBinary(CLIENT socket, byte[] byteArray);
    protected abstract void OnSocketMessage(CLIENT socket,MSG message);

    protected abstract void OnSocketOpen(CLIENT socket);
    protected abstract void OnSocketClose(CLIENT socket);



}