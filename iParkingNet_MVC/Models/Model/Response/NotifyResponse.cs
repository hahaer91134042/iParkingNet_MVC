using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// NotifyResponse 的摘要描述
/// </summary>
public class NotifyResponse : ApiAbstractModel
{
    public NotifyType Type { get; set; }
    public object Content { get; set; }
}