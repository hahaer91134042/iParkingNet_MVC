using DevLibs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

/// <summary>
/// WebConnect 的摘要描述
/// </summary>
public class WebConnect
{
    public interface IBody
    {
        string body();
    }
    public interface IConfig
    {
        string url();
        Method method();
        ClientContentType contentType();
        void setHeader(WebHeaderCollection header);
    }
    public enum Method
    {
        GET,
        POST
    }
    public static WebConnect CreatBy(IConfig config)
    {
        var request = WebRequest.CreateHttp(config.url());
        request.Method = config.method().ToString();

        var connect = new WebConnect(request);
        connect.ContentType = config.contentType();
        config.setHeader(connect.Header);
        if (config is IBody)
            connect.setBody((config as IBody).body());

        return connect;
    }
    public static WebConnect CreatGet(string url)
    {
        var request = WebRequest.CreateHttp(url);
        request.Method = "GET";
        return new WebConnect(request);
    }
    public static WebConnect CreatPost(string url)
    {
        var request = WebRequest.CreateHttp(url);
        request.Method = "POST";
        return new WebConnect(request);
    }

    private HttpWebRequest request;
    public ClientContentType ContentType { set { request.ContentType = value.Type; } get { return new ClientContentType(request.ContentType); } }
    public long ContentLength { get { return request.ContentLength; } set { request.ContentLength = value; } }
    public HttpStatusCode StatusCode { get; set; }
    public WebHeaderCollection Header { get { return request.Headers; } }

    public WebConnect(HttpWebRequest request)
    {
        this.request = request;
    }

    //public WebConnect XmlContent()
    //{
    //    request.ContentType = "text/xml;charset=utf-8";
    //    return this;
    //}

    public WebConnect setBody(string data)
    {
        setBody(Encoding.UTF8.GetBytes(data));
        return this;
    }

    public WebConnect setBody(byte[] data)
    {
        ContentLength = data.Length;
        //Log.d($"Request ssl->{ServicePointManager.SecurityProtocol}");
        var stream = request.GetRequestStream();
        stream.Write(data, 0, data.Length);
        //stream.Flush();
        stream.Close();
        return this;
    }

    public T Connect<T>()
    {
        return JsonConvert.DeserializeObject<T>(Connect());
    }
    public string Connect()
    {        
        using (var httpResponse = (HttpWebResponse)request.GetResponse())
        {
            StatusCode = httpResponse.StatusCode;
            //Encoding.UTF8
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }        
    }

}