using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SocketMsg 的摘要描述
/// </summary>
public class SocketReceiveMsg:ApiAbstractModel
{   
    public string time { get; set; }
    public string method { get { return socketMethod.ToString(); } set { socketMethod = value.toEnum<SocketMethod>(); } }
    //public string target { get; set; }
    public object request { get; set; }
    public SocketMethod socketMethod = SocketMethod.SendTo;
}