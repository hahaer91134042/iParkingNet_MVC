using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// EcPayExt 的摘要描述
/// </summary>
public static class EcPayExt
{
    public static long toUnixSecStamp(this DateTime time)
    {
        return Convert.ToInt64(time.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
    }
    public static string toEcPayItemName(this List<string> list)
    {
        var builder = new StringBuilder();
        list.ForEach(name =>
        {
            builder.Append(name + "#");
        });
        if (builder.Length > 0)
            builder.Remove(builder.Length - 1, 1);
        return builder.ToString();
    }
    public static string encodeSHA256(this string text)
    {
        var algorithm = HashAlgorithm.Create("SHA256");
        var hashByteArray = algorithm.ComputeHash(Encoding.UTF8.GetBytes(text));
        return BitConverter.ToString(hashByteArray).Replace("-", "");
    }
    public static List<PropertyInfo> filterEcPayProperty<T>(this T input,bool isIncludeCheckCode=false)
    {
        return (from prop in input.GetType().GetProperties()
                where prop.IsDefined(typeof(EcPayFeature), true)
                where isIncludeCheckCode ? true : !(prop.GetCustomAttributes(typeof(EcPayFeature), true).FirstOrDefault() as EcPayFeature).isCheckCode
                orderby prop.Name
                select prop).ToList();
    }
    public static string getCheckCode<T>(this T request)where T:IEcPayCheck
    {
        //要先生成確認碼 先找出非確認碼的欄位 按字母排列
        var propertys = request.filterEcPayProperty();

        var pairs = new Dictionary<string, string>();
        pairs.Add("HashKey", request.hashKey());
        propertys.ForEach(prop =>
        {
            pairs.Add(prop.Name, prop.GetValue(request, null).ToString());
        });
        pairs.Add("HashIV", request.hashIV());

        var builder = new StringBuilder();
        pairs.Foreach((k, v) =>
        {
            builder.Append(k + "=" + v + "&");
        });

        if (builder.Length > 0)
            builder.Remove(builder.Length - 1, 1);

        //return builder.ToString();
        var code= HttpUtility.UrlEncode(builder.ToString()).ToLower();

        return code.encodeSHA256().ToUpper();
    }

    public static T toEcPayObj<T>(this NameValueCollection input)
    {
        var obj = Activator.CreateInstance<T>();
        var properties = obj.filterEcPayProperty(true);
        properties.ForEach(p =>
        {
            var name = p.Name;
            if (input.AllKeys.Contains(name))
                p.SetValue(obj, Convert.ChangeType(input[name],p.PropertyType),null);
        });
        return obj;
    }

    public static bool checkEcPay<T>(this T input)where T:IEcPayCheck
    {
        try
        {
            var code = input.filterEcPayProperty(true)
                .FirstOrDefault(p => (p.GetCustomAttributes(typeof(EcPayFeature), true).FirstOrDefault() as EcPayFeature).isCheckCode)
                .GetValue(input,null).ToString();

            return code == input.getCheckCode();
        }
        catch (Exception) { }
        return false;
    }
}