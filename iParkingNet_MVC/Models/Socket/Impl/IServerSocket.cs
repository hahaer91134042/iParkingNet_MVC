using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 面向服務器的socket client(Fleck)
/// 主要是開啟一個socket 服務讓其他用戶端對接
/// </summary>
public abstract class IServerSocket<CLIENT,MSG>
{
    protected string url;
    public IServerSocket(string url)
    {
        this.url = url;
        InitSocket();
    }

    protected abstract void InitSocket();

    protected abstract void OnSocketBinary(CLIENT socket, byte[] byteArray);
    protected abstract void OnSocketMessage(CLIENT socket,MSG message);

    protected abstract void OnSocketOpen(CLIENT socket);
    protected abstract void OnSocketClose(CLIENT socket);


    /// <summary>
    /// Sec-WebSocket-Protocol 設定要帶入的值
    /// </summary>
    protected virtual string[] subProtocols { get => new string[] { }; }

}