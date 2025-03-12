using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;

/// <summary>
/// InvoiceConnecter 的摘要描述
/// </summary>
namespace Eki_NewebPay
{
    public class NewebPayInvoice
    {
        /*
         1=應稅
         2=零稅率
         3=免稅
         9=混合應稅與免稅或零稅率(限發票種類為B2C，Category=B2C時使用)
        */
        public const string TaxType_Taxable = "1";
        public const string TaxType_MixTaxable = "9";

        /// <summary>
        /// 即時開立發票
        /// </summary>
        public const string Status_instant = "1";
        /// <summary>
        /// 等待觸發開立發票(須於確認要開立時，再發動觸發)
        /// </summary>
        public const string Status_wait = "0";
        /// <summary>
        /// 預約自動開立發票(須指定預計開立日期)
        /// </summary>
        public const string Status_auto = "3";

        public const string Category_B2B = "B2B";
        public const string Category_B2C = "B2C";
        public const string PrintFlag_Y = "Y";
        public const string PrintFlag_N = "N";

        /// <summary>
        /// 手機條碼載具
        /// </summary>
        public const string CarrierType_phone = "0";
        /// <summary>
        /// 自然人憑證
        /// </summary>
        public const string CarrierType_person = "1";
        public const string CarrierType_ezPay = "2";

        public class Config
        {
            public const string RespondType = "String";
            public const string Version = "1.4";

            public const double TaxRate = 5.0;
        }


        private INewebPayConfig config;
        internal NewebPayInvoice(INewebPayConfig c)
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
        public NewebPayInvoiceReturn Post(NewebPayInvoiceModel model)
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

            connect.setBody(postParams.ToString());

            //result = result.Replace("\", "");

            return NewebPayInvoiceReturn.Parser.parse(connect.Connect());
            //return JsonConvert.DeserializeObject<InvoiceReturn>(result);

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
        //private async Task<string> post()
        //{
        //    var config = PayConfig.Invoice.config();
        //    var parser = NewebPayInfoParser.Parse(Model);
        //    var info = config.EncryptAES256(parser.GetInfo());
        //    var value = new
        //    {
        //        MerchantID_ = config.merchantID(),
        //        PostData_ = info
        //    };
        //    var client = new HttpClient();
        //    var content = new StringContent(value.toJsonString());
        //    //var auth = "qdaiciDiyMaTjxMt, 74026b3dc2c6db6a30a73e71cdb138b1e1b5eb7a97ced46689e2d28db1050875";
        //    ////-H "Authorization: qdaiciDiyMaTjxMt, 74026b3dc2c6db6a30a73e71cdb138b1e1b5eb7a97ced46689e2d28db1050875" \
        //    //client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", auth);
        //    //-H 'Content-Type: application/json' \
        //    client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

        //    var response = await client.PostAsync(config.url(), content);

        //    var responseString = await response.Content.ReadAsStringAsync();

        //    return responseString;
        //}
    }
}
