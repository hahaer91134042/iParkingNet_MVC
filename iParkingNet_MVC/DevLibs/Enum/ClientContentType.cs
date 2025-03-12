using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ClientContentType 的摘要描述
/// </summary>
public class ClientContentType
{
    public static ClientContentType ApplicationJson = new ClientContentType("application/json");
    public static ClientContentType TextXml = new ClientContentType("text/xml;charset=utf-8");
    public static ClientContentType FormPost = new ClientContentType("application/x-www-form-urlencoded");

    public string Type;
    public ClientContentType(string v)
    {
        this.Type = v;
    }
}