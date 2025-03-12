using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevLibs;
using Newtonsoft.Json;

/// <summary>
/// LinePayConfig 的摘要描述
/// </summary>
namespace Eki_LinePayApi_v3
{
    public class LinePay
    {
        public class Config
        {
            //測試用
            public const string BaseUrl = "https://sandbox-api-pay.line.me";
            //正式用
            //public const string BaseUrl = "https://api-pay.line.me";
        }


        //public class Url
        //{
        //    public static Url Request = new Url(Config.BaseUrl, Path.RequestApi);
        //    public static Url Confirm = new Url(Config.BaseUrl, Path.ConfirmApi);

        //    public string baseUrl;
        //    public Uri baseUri { get => new Uri(baseUrl); }
        //    public Path path;
        //    private Url(string b, Path p)
        //    {
        //        baseUrl = b;
        //        path = p;
        //    }

        //    public Url addPathParams(params string[] param)
        //    {
        //        path.addParams(param);
        //        return this;
        //    }

        //    public override string ToString()
        //    {
        //        return $"{baseUrl}{path}";
        //    }
        //}

        public class Path
        {
            public static Path RequestApi = new Path("payments/request");
            //要帶入transcationId
            public static Path ConfirmApi = new Path("payments/{0}/confirm");

            private string _ver;
            private string path;
            private string[] parameters = new string[0];
            private Path(string p,string ver="v3")
            {
                path = p;
                _ver = ver;
            }

            public Path addParams(params string[] param)
            {
                parameters = param;
                return this;
            }

            public override string ToString()
            {
                return string.Format($"/{_ver}/{path}", parameters);
            }
        }

        public class Client
        {
            public ILinePayConfig config;

            //public string nonce = Guid.NewGuid().ToString();
            //public Url url;
            //public object data;

            private readonly HttpClient _client;
            public readonly JsonSerializerSettings SerializerSettings;

            public Client(ILinePayConfig c)
            {
                config = c;
                //url = u;
                //data = d;
                //Log.d($"LinePay channelID->{c.ChannelID} \n" +
                //    $"Key->{c.SecretKey}" +
                //    $"BaseUrl->{Config.BaseUrl}");

                _client = new HttpClient { BaseAddress = new Uri(Config.BaseUrl) };
                SerializerSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            }

            /// <summary>
            /// 請求付款
            /// </summary>
            /// <param name="reserve"></param>
            /// <returns></returns>
            public async Task<LinePayResult.Reserve> ReserveAsync(LinePayReserve reserve)
            {
                var path = Path.RequestApi.ToString();
                var uuid = Guid.NewGuid().ToString();
                var value = JsonConvert.SerializeObject(reserve, SerializerSettings);
                var signature = Helper.Encrypt(path, value, uuid, config.SecretKey);
                //var signature= HmacSHA256($"{config.SecretKey}{url.path}{value}{uuid}", config.SecretKey);


                this.SetHttpHeader(uuid, signature);
                return await Post<LinePayResult.Reserve>(value,path);
            }

            /// <summary>
            /// 確認付款
            /// </summary>
            /// <param name="confirm"></param>
            /// <param name="transactionId"></param>
            /// <returns></returns>
            public async Task<LinePayResult.Confirm> ConfirmAsync(LinePayConfirm confirm,long transactionId)
            {
                var path = Path.ConfirmApi.addParams(transactionId.ToString()).ToString();
                var uuid = Guid.NewGuid().ToString();
                var value = JsonConvert.SerializeObject(confirm, SerializerSettings);
                var signature = Helper.Encrypt(path, value, uuid, config.SecretKey);

                this.SetHttpHeader(uuid, signature);
                return await Post<LinePayResult.Confirm>(value, path);
            }

            /// <summary>
            /// 設定 Request HttpHeader
            /// </summary>
            /// <param name="uuid">UUID or timestamp(時間戳)</param>
            /// <param name="signature">HMAC Base64 簽章</param>
            private void SetHttpHeader(string uuid, string signature)
            {
                //Log.d($"LinePay Header nonce->{uuid} \n" +
                //$"Signature->{signature} \n");

                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.TryAddWithoutValidation("X-LINE-ChannelId", config.ChannelID);
                _client.DefaultRequestHeaders.TryAddWithoutValidation("X-LINE-Authorization-Nonce", uuid);
                _client.DefaultRequestHeaders.TryAddWithoutValidation("X-LINE-Authorization", signature);
                _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            }


            private async Task<TResult> Post<TResult>(string body, string requestUri)
            {
                //Log.d($"LinePay Post url->{requestUri}\n" +
                //    $" data->{body}\n");

                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(body, Encoding.UTF8, "application/json")
                };

                var response = await _client.SendAsync(httpRequestMessage);

                //Log.d($"LinePay result code->{response.IsSuccessStatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
                    //Log.d($"LinePay result->{result.toJsonString()}");

                    return result; 
                }                    
                else
                    throw new Exception(await response.Content.ReadAsStringAsync());
            }

            private async Task<TResult> Get<TResult>(string requestUri)
            {
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

                var response = await _client.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<TResult>(await response.Content.ReadAsStringAsync());
                else
                    throw new Exception(await response.Content.ReadAsStringAsync());
            }


            //public T getResult<T>()
            //{
            //    var valueStr = JsonConvert.SerializeObject(data);

            //    string Signature = HmacSHA256($"{config.SecretKey}{url.path}{valueStr}{nonce}", config.SecretKey);

            //    Log.print($"LinePay Url->{url} \n" +
            //        $"Config->{config.toJsonString()} \n" +
            //        $"nonce->{nonce} \n" +
            //        $"Signature->{Signature} \n" +
            //        $"Value->{valueStr}");


            //    var request = WebRequest.CreateHttp(Url.Reserve.ToString());
            //    request.Method = "POST";
            //    request.ContentType = "application/json";
            //    request.Headers.Add("X-LINE-ChannelId", config.ChannelID);
            //    request.Headers.Add("X-LINE-Authorization-Nonce", nonce);
            //    request.Headers.Add("X-LINE-Authorization", Signature);
            //    request.Headers.Add("X-LINE-MerchantDeviceProfileId", "DUMMY");



            //    var body = Encoding.UTF8.GetBytes(valueStr);

            //    using (var stream = request.GetRequestStream())
            //    {
            //        stream.Write(body, 0, body.Length);
            //    }

            //    using (var response = (HttpWebResponse)request.GetResponse())
            //    using (var respStream = new StreamReader(response.GetResponseStream()))
            //    {
            //        var result = respStream.ReadToEnd();
            //        Log.print($"LinePay Result->{result}");
            //        return JsonConvert.DeserializeObject<T>(result);
            //    }
            //}

            //public async Task<T> getResultAsync<T>()
            //{
            //    return await Task.Run(() => getResult<T>());
            //}

            //private static string HmacSHA256(string message, string key = "")
            //{
            //    var encoding = new System.Text.UTF8Encoding();
            //    byte[] keyByte = encoding.GetBytes(key);
            //    byte[] messageBytes = encoding.GetBytes(message);
            //    using (var hmacsha256 = new HMACSHA256(keyByte))
            //    {
            //        byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            //        return Convert.ToBase64String(hashmessage);
            //    }
            //}
        }

        public class Currency
        {
            public static Currency TWD = new Currency("TWD");
            public static Currency USD = new Currency("USD");
            public static Currency JPY = new Currency("JPY");
            public static Currency THB = new Currency("THB");

            private string key;
            private Currency(string k)
            {
                key = k;
            }
            public override string ToString() => key;
        }


        internal static class Helper
        {

            /// <summary>
            /// 
            /// </summary>
            /// <param name="message">( ChannelSecret + apiUrl + requestJson + nonce)</param>
            /// <param name="key">ChannelSecret</param>
            /// <returns></returns>
            public static string Encrypt(string path,string value,string nonce, string key)
            {
                var message = string.Concat(key,path,value,nonce);
                var encoding = new System.Text.UTF8Encoding();
                var keyByte = encoding.GetBytes(key);
                var messageBytes = encoding.GetBytes(message);
                using (var hmac256 = new HMACSHA256(keyByte))
                {
                    var hashMessage = hmac256.ComputeHash(messageBytes);
                    return Convert.ToBase64String(hashMessage);
                }
            }
        }
    }
}
