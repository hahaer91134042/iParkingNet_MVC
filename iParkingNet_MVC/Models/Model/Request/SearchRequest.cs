using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CheckOutRequest;

/// <summary>
/// SearchRequest 的摘要描述
/// </summary>
public class SearchRequest: RequestAbstractModel
{
    public List<int> id { get; set; }
    public List<string> serNum { get; set; }
    public string token { get; set; }
    public string time { get; set; }//yyyy-MM-dd HH:mm:ss
    public List<TimeSpan> times { get; set; }
    public TimeSpan timeSpan { get; set; } = null;
    public Invoice invoice { get; set; }
    public double lat { get; set; } = 0;
    public double lng { get; set; } = 0;
    public bool isVerify { get; set; } = false;//是否審核
    public string text { get; set; }="";
    public string serial { get; set; }
    public object data { get; set; }

    public List<OpenSet> timesToOpenList(string format=ApiConfig.DateTimeFormat)
    {
        try
        {            
            return new List<OpenSet>().Also(list =>
            {
                times.ForEach(span =>
                {
                    list.Add(new OpenSet(span.start.toDateTime(format, b => { if (!b) throw new InputFormatException(); }),
                        span.end.toDateTime(format, b => { if (!b) throw new InputFormatException(); })));
                });
            });
        }
        catch (Exception)
        {
            throw new InputFormatException();
        }
    }

    public bool isTimeSpanOrEmpty()
    {
        if (timeSpan.isNullOrEmpty())
            return true;
        return timeSpan.start.isDateTime(ApiConfig.DateTimeFormat) && timeSpan.end.isDateTime(ApiConfig.DateTimeFormat);
    }

    public bool isTimeOrEmpty()
    {
        //允許空字串 假如有填入在檢查時間格式
        if (time.isNullOrEmpty())
            return true;
        return time.isDateTime(ApiConfig.DateTimeFormat);
    }
    public bool isIdEmpty()
    {        
        if (id != null)
            return id?.Count <= 0;
        return true;
    }

    public bool isSerNumEmpty()
    {
        if (serNum != null)
            return serNum?.Count <= 0;
        return true;
    }
    public string getUserUniqueId()
    {
        try
        {
            var auth = JwtBuilder.GetDecoder()
                           .setToken(token)
                           .decode();
            return auth.user;
        }
        catch(Exception e)
        {
        }
        return null;
    }

    public T getData<T>()
    {
        return data.toObj<T>();
    }

    public class TimeSpan
    {
        public string start { get; set; } = "";
        public string end { get; set; } = "";
    }
    public class ExtendOrderEnd
    {
        public string serNum { get; set; } = "";
        public string time { get; set; } = "";
    }
}