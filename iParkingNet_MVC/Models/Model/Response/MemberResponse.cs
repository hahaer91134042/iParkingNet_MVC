using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// MemberResponse 的摘要描述
/// </summary>
public class MemberResponse:ApiAbstractModel
{
    public string Phone { get; set; } = "";
    public string NickName { get; set; } = "";
}