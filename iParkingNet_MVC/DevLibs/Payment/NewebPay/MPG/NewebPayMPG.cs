using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// NewebPayConnecter 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public class NewebPayMPG
    {
        public class Config
        {
            //public const string TestUrl = "https://ccore.newebpay.com/MPG/mpg_gateway";
            //public const string Url = "https://core.newebpay.com/MPG/mpg_gateway";
            //public const string HashKey = "MFcY951ZD7Z7r0CS6kEZSRlCNBlIJMvy";//測試
            //public const string HashKey = "EFWfsBOzJwQK3dbdp4mkRWF4VLdhWYDH";//正式
            //public const string HashIV = "CSldFitbiFogBokP";//測試
            //public const string HashIV = "PQmVVQRy1jl4a7dC";//正式
            //public const string MerchantID = "MS36975845";//測試 
            //public const string MerchantID = "MS3374995046";//正式
            public const string Version = "1.7";
            public const string RespondType = "JSON";
            public const string LangType_Tw = "zh-tw";
            public const string LangType_En = "en";
            public const string LoginType = "0";
            public const string CREDIT = "1";
            public const string LINEPAY = "1";
            //public const string BARCODE = "1";
        }

        private INewebPayConfig config;
        internal NewebPayMPG(INewebPayConfig c)
        {
            config = c;
        }

        public void Post(NewebPayMPGModel model)
        {
            //先設定固定的值上去
            //Model.MerchantID = PayConfig.藍新.MerchantID;
            //Model.Version = PayConfig.藍新.Version;
            //Model.RespondType = PayConfig.藍新.RespondType;
            //Model.LoginType = PayConfig.藍新.LoginType;
            //Model.CREDIT = PayConfig.藍新.CREDIT;

            //var config = PayConfig.藍新.config();

            model.MerchantID = config.merchantID();
            //Log.print($"Post MPG Model->{model.toJsonString()}");

            var parser = NewebPayInfoParser.Parse(model);
            var info = config.EncryptAES256(parser.GetInfo());
            var sha256 = config.getHashSha256(info);

            RemotePost remotePost = new RemotePost();
            //remotePost.Url = PayConfig.藍新.TestUrl;
            remotePost.Url = config.url();
            remotePost.Add("MerchantID", config.merchantID());
            //remotePost.Add("MerchantID", PayConfig.藍新.MerchantID);
            remotePost.Add("TradeInfo", info);
            remotePost.Add("TradeSha", sha256);
            remotePost.Add("Version", NewebPayMPG.Config.Version);

            remotePost.Post();
        }

    }
}
