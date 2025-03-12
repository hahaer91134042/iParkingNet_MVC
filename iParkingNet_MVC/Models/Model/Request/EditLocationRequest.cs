using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EditLocationRequest 的摘要描述
/// </summary>
public class EditLocationRequest:RequestAbstractModel,ApiFeature_v2.Request
{
   
    public int id { get; set; }
    public LocationInfoRequest info { get; set; }
    public ReservaConfigRequest config { get; set; }
    //v2版api才有使用
    public ChargeSocketRequest socket { get; set; } = new ChargeSocketRequest();


    public override bool cleanXss()
    {
        return info.cleanXss() && config.cleanXss();
    }

    public override bool isValid()
    {
        return id>0 && info.isValid() && config.isValid() && cleanXss();
        //return id > 0 && info.isValid() && config.isValid() && cleanXss();
    }

    public bool isValid_v2()
    {
        //&& socket.isValid_v2()
        return id > 0 && info.isValid_v2() && config.isValid() && cleanXss() && socket.isValid_v2();
    }
}