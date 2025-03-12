using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// IScheme 的摘要描述
/// </summary>
public abstract class IScheme
{
    protected Uri uri;

    public IScheme()
    {
        uri = new Uri($"{scheme()}://{host()}/{path()}?action={action()}");
    }

    public abstract string scheme();
    public abstract string host();
    public abstract string path();
    public abstract string action();
    public virtual Uri Uri(params QueryPair[] pairs)
    {
        var builder = new StringBuilder(uri.ToString());
        foreach(var pair in pairs)
        {
            builder.Append($"&{pair.key}={pair.value}");
        }

        return new Uri(builder.ToString());
    }


    public class QueryPair 
    {
        public string key;
        public string value;
        public QueryPair(string k,string v)
        {
            key = k;
            value = v;
        }
    }
}