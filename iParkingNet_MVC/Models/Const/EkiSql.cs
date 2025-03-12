using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// EkiSql 的摘要描述
/// </summary>
public class EkiSql
{
    public static PpypSql ppyp = new PpypSql();
    public class PpypSql : ISql
    {
        public IDbConfig dbConfig() => new iParkingNetDbConfig();

        public int timeOut() => 10000;
    }

    public class iParkingNetDbConfig : IDbConfig
    {
        public string connetString() => iParkingNet_MVC.Properties.Settings.Default.cnnStr;
    }
}