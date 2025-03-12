using DevLibs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Web;

/// <summary>
/// NewebPayCreditCard 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public class NewebPayCreditCard
    {
        public class Config
        {
            public const string Version = "1.6";
            //public const string ResponseType = "String";
            public const string ResponseType = "JSON";

        }

        private INewebPayConfig config;
        internal NewebPayCreditCard(INewebPayConfig c)
        {
            config = c;
        }

        private class ConnectConfig : WebConnect.IConfig
        {
            private INewebPayConfig config;
            public ConnectConfig(INewebPayConfig c) => config = c;
            public ClientContentType contentType() => ClientContentType.FormPost;
            public WebConnect.Method method() => WebConnect.Method.POST;
            public void setHeader(WebHeaderCollection header)
            {
            }
            public string url() => config.url();
        }


        public NewebPayCreditReturn Post(NewebPayCreditModel model)
        {

            var parser = NewebPayInfoParser.Parse(model);
            var info = config.EncryptAES256(parser.GetInfo());
            //var value = new
            //{
            //    MerchantID_ = config.merchantID(),
            //    PostData_ = info
            //};

            var connect = WebConnect.CreatBy(new ConnectConfig(config));


            //var request = WebRequest.Create(config.url());
            //request.Method = "POST";
            //request.ContentType = "application/x-www-form-urlencoded";

            //必須透過ParseQueryString()來建立NameValueCollection物件，之後.ToString()才能轉換成queryString
            NameValueCollection postParams = HttpUtility.ParseQueryString(string.Empty);
            postParams.Add("MerchantID_", config.merchantID());
            postParams.Add("PostData_", info);
            postParams.Add("Pos_", Config.ResponseType);
            //Log.d($"Credit Card post->{postParams}");
            connect.setBody(postParams.ToString());

            //Log.print($"Credit response->{connect.Connect()}");

            return connect.Connect<NewebPayCreditReturn>();

            //using (var reqStream = request.GetRequestStream())
            //{
            //    //要發送的字串轉為byte[] 
            //    byte[] byteArray = Encoding.UTF8.GetBytes(postParams.ToString());
            //    reqStream.Write(byteArray, 0, byteArray.Length);
            //}//end using
            // //API回傳的字串
            //string responseStr = "";
            ////發出Request
            //using (var response = request.GetResponse())
            //{
            //    using (var sr = new StreamReader(response.GetResponseStream()))
            //    {
            //        responseStr = sr.ReadToEnd();
            //    }//end using  
            //}

            //return responseStr;    
        }

    }
}
