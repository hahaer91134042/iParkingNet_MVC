using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

/// <summary>
/// WebUtil 的摘要描述
/// </summary>
public class WebUtil
{
    private const string webURL = "webURL";
    private const string mailFrom = "mailFrom";
    private const string mailTo = "mailTo";
    private const string mailServer = "mailServer";
    private const string fileDir = "file_dir";
    private const string mailUid = "mailUid";
    private const string mailPwd = "mailPwd";
    private const string tokenLifeDay = "TokenLifeDay";

    public static string getConfigAppSet(string key)
    {
        return ConfigurationManager.AppSettings[key];
    }
    public static string getMailUserName()
    {
        return getConfigAppSet(mailUid);
    }
    public static string getMailPwd()
    {
        return getConfigAppSet(mailPwd);
    }
    public static int getTokenDay()
    {
        return Convert.ToInt32(getConfigAppSet(tokenLifeDay));
    }
    public static string getMailServerIp()
    {
        return getConfigAppSet(mailServer);
    }
    public static string getMailFrom()
    {
        return getConfigAppSet(mailFrom);
    }
    public static string getWebURL()
    {
        return getConfigAppSet(webURL);
    }
    public static string getFileDir()
    {
        return getConfigAppSet(fileDir);
    }
    /**
     * 網站實體根目錄
     **/
    public static string getWebBaseDirectoryPath()
    {
        return System.AppDomain.CurrentDomain.BaseDirectory;
    }
    /**
     * 網站實體根目錄
     **/
    public static string getServerMapPath()
    {
        return HttpContext.Current.Server.MapPath("~");
    }
    /**
     *該檔案在伺服器上完整實體路徑 
     ***/
    public static string getInstanceFilePath()
    {
        return HttpContext.Current.Request.PhysicalPath;
    }
    public static string GetUserIP()
    {
        HttpContext context = HttpContext.Current;
        string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        if (!string.IsNullOrEmpty(ipAddress))
        {
            string[] addresses = ipAddress.Split(',');
            if (addresses.Length != 0)
            {
                return addresses[0];
            }
        }

        return context.Request.ServerVariables["REMOTE_ADDR"];
    }
    public static string GetTempDirectory()
    {
        return HttpContext.Current.Server.MapPath(@"~\upload\temp");
    }
}