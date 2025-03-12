using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Eki_LinePayApi_v3;

/// <summary>
/// EkiLinePayConfig 的摘要描述
/// </summary>
public class EkiLinePay
{
    //測試
    public static ILinePayConfig Config = new Test();    

    //正式
    //public static ILinePayConfig Config = new EkiLine();


    private class Test : ILinePayConfig
    {
        
        public string ChannelID => "1657223985";
        public string SecretKey => "de79880e814e738da580aef94ad26b8a";

        public string cancelUrl(params string[] args)
        {
            return $"{ApiConfig.Url_iParkingNet}/page/LinePay/Cancel.aspx";
        }

        public string confirmUrl(params string[] args)
        {
            return (ApiConfig.Url_iParkingNet+"/page/LinePay/Confirm.aspx?ord={0}").format(args);
        }
    }

    private class EkiLine : ILinePayConfig
    {
        public string ChannelID => "1657044693";
        public string SecretKey => "fc0f2ed95baf594edfef8a524c9559f5";

        public string cancelUrl(params string[] args)
        {
            return $"{ApiConfig.Url_PpypApi}/page/LinePay/Cancel.aspx";
        }

        public string confirmUrl(params string[] args)
        {
            return (ApiConfig.Url_PpypApi + "/page/LinePay/Confirm.aspx?ord={0}").format(args);
        }
    }


}