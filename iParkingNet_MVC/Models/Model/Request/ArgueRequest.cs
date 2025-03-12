using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ArgueRequest 的摘要描述
/// </summary>
public class ArgueRequest:RequestAbstractModel
{
    public string serial { get; set; } = "";//目前是訂單序號
    public int source { get; set; }
    public int type { get; set; }

    public string text { get; set; } = "";

    public double lat { get; set; } = 0d;
    public double lng { get; set; } = 0d;

    public override bool isValid()
    {
        try
        {
            serial.cleanXss();
            text.cleanXss();
            if (serial.isNullOrEmpty()&&text.isNullOrEmpty())
                return false;

            source.toEnum<ArgueSource>();//轉換看看會不會出現錯誤
            type.toEnum<ArgueType>();

            return true;
        }
        catch (Exception)
        {

        }
        return false;
    }
}