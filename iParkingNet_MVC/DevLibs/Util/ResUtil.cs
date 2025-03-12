using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ResUtil 的摘要描述
/// </summary>
public class ResUtil
{
    internal const string ApiResource = "ApiResource";
    public static string GetApiRes(LanguageFamily lan, string key)
    {
        return GetApiRes(lan.ToString(), key);
    }
    public static string GetApiRes(string lan, string key)
    {
        return HttpContext.GetGlobalResourceObject(ApiResource + lan, key).ToString();
    }
    public static string GetWebRes(string lan, string key)
    {
        return HttpContext.GetGlobalResourceObject(lan, key).ToString();
    }
}