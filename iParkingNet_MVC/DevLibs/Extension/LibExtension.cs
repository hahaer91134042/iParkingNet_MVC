using DevLibs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// LibExtension 的摘要描述
/// </summary>
public static class LibExtension
{
    // Kotlin: fun <T, R> T.let(block: (T) -> R): R
    public static R Let<T, R>(this T self, Func<T, R> block)
    {
        return block(self);
    }

    // Kotlin: fun <T> T.also(block: (T) -> Unit): T
    public static T Also<T>(this T self, Action<T> block)
    {
        block(self);
        return self;
    }
    public static bool isNotEmpty<T>(this List<T> list)
    {
        if (list == null)
            return false;
        if (list.Count < 1)
            return false;
        return true;
    }
    public static void isNull(this object input,Action back)
    {
        if (input == null)
            back();
    }
    public static void notNull<T>( this T input,Action<T> back)
    {
        if (input != null)
            back(input);
    }

    #region Number Extension
    public static double toDouble(this string input) => Convert.ToDouble(input);
    public static double toDouble(this int input) => Convert.ToDouble(input);
    public static decimal toDecimal(this double input)
    {
        return Convert.ToDecimal(input);
    }
    public static double toDouble(this decimal input)
    {
        return Convert.ToDouble(input);
    }
    //decimal 四捨五入
    public static decimal Round(this decimal input, int digits = 0) => Math.Round(input, digits, MidpointRounding.AwayFromZero);
    //decimal 無條件進位
    public static decimal Ceiling(this decimal input) => Math.Ceiling(input);
    public static double Round(this double input, int digits = 0) => Math.Round(input, digits, MidpointRounding.AwayFromZero);
    public static double Ceiling(this double input) => Math.Ceiling(input);
    public static int toInt(this decimal input)
    {
        return Convert.ToInt32(input);
    }
    public static long toLong(this decimal value)
    {
        try
        {
            return Convert.ToInt64(value);
        }
        catch (Exception)
        {
            return 0L;
        }
    }
    #endregion

    #region DateTime extension
    public static bool monthEqual(this DateTime ori, DateTime other) => (ori.Year == other.Year) && (ori.Month == other.Month);
    public static DateTime toMonthMinDate(this DateTime input) => new DateTime(input.Year, input.Month, 1);
    public static DateTime toMonthMaxDate(this DateTime input) 
    {
        if (input.Month == 12)
        {
            return new DateTime(input.Year + 1, 1, 1).AddDays(-1);
        }
        return new DateTime(input.Year, input.Month + 1, 1).AddDays(-1);
    }
    public static bool sameDay(this DateTime ori, DateTime other) => (ori.Date - other.Date).TotalDays == 0;
    public static string toSerialNumber(this DateTime input)
    {
        return input.ToString("yyyyMMddhhmmssfff");
    }
    public static long toStamp(this DateTime input)
    {
        return TimeUtil.toStamp(input);
    }
    //這是避免最小值超出sql的範圍
    public static DateTime toSqlTime(this DateTime input)
    => input < SqlDateTime.MinValue.Value ? SqlDateTime.MinValue.Value : input;
    public static DateTime parse24Hour(this string input)
    {
        var wrapped = Regex.Replace(input, @"24:(\d\d:\d\d)$", "00:$1");
        var res = DateTime.Parse(wrapped);
        return wrapped != input
            ? res.AddDays(1)
            : res;
    }
    public static long toStamp(this string input, string format)
    {
        var isOk = false;
        var time = input.toDateTime(format, ok =>
        {
            isOk = ok;
        });
        if (isOk)
            return time.toStamp();
        return 0L;
    }
    public static long toStamp(this string input)
    {
        return input.toDateTime().toStamp();
    }
    public static bool isDateTime(this string input, string format = "yyyy-MM-dd HH:mm:ss")
    {
        DateTime time;
        return DateTime.TryParseExact(input, format, null, System.Globalization.DateTimeStyles.None, out time);
    }
    public static DateTime toDateTime(this string input, string format, Action<bool> back = null)
    {

        DateTime time;
        var ok = DateTime.TryParseExact(input.Trim(), format, null, System.Globalization.DateTimeStyles.None, out time);
        back?.Invoke(ok);
        return time;
    }
    public static DateTime toDateTime(this string input)
    {
        return input.parse24Hour();
    }
    //不能解析毫秒
    public static TimeSpan toTimeSpan(this string input, string format = "HH:mm:ss", char separator = ':')
    {
        try
        {
            var values = input.Split(separator);
            var formatDigest = format.Split(separator);
            //無法解析出有用的
            if (values.Length < 1)
                throw new ArgumentNullException();
            //格式錯誤
            if (values.Length != formatDigest.Length)
                throw new FormatException();

            var dic = new Dictionary<string, int>();

            for (int i = 0; i < formatDigest.Length; i++)
            {
                dic.Add(formatDigest[i], values[i].toInt());
            }

            var day = (from k in dic.Keys
                       where k.ToLower() == "dd"
                       select dic[k]).FirstOrDefault();
            var hour = (from k in dic.Keys
                        where k.ToLower() == "hh"
                        select dic[k]).FirstOrDefault();
            var min = (from k in dic.Keys
                       where k.ToLower() == "mm"
                       select dic[k]).FirstOrDefault();
            var sec = (from k in dic.Keys
                       where k.ToLower() == "ss"
                       select dic[k]).FirstOrDefault();
            var milliseconds = (from k in dic.Keys
                                where k.ToLower() == "ff"
                                select dic[k]).FirstOrDefault();

            return new TimeSpan(day, hour, min, sec, milliseconds);
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    public static string toString(this DateTime time, string format = TimeUtil.DateTimeFormat) => time.ToString(format);

    #endregion

    #region FileInfo
    public static string convertFileName(this FileInfo input, string newName)
    {
        return newName + input.Extension;
    }
    #endregion

    #region string extension    
    public static string toString(this Guid guid) => guid.ToString().ToUpper();
    public static string toBase64(this string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));

    public static string decodeBase64(this string strBase64)
    {
        return Encoding.UTF8.GetString(Convert.FromBase64String(strBase64));
    }
    public static string maxString(this string input, int max) => input.Length <= max ? input : input.Substring(0, max);
    public static string toBig5(this string input)
    {
        Encoding big5 = Encoding.GetEncoding("big5");
        byte[] strUtf8 = Encoding.Unicode.GetBytes(input);
        byte[] strBig5 = Encoding.Convert(Encoding.Unicode, big5, strUtf8);
        return big5.GetString(strBig5);
    }
    /// 轉全形的函數(SBC case)
    ///
    ///任一字元串
    ///全形字元串
    ///
    ///全形空格為12288，半形空格為32
    ///其他字元半形(33-126)與全形(65281-65374)的對應關係是：均相差65248
    ///
    public static String toSBC(this string input)
    {
        // 半形轉全形：
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 32)
            {
                c[i] = (char)12288;
                continue;
            }
            if (c[i] < 127)
                c[i] = (char)(c[i] + 65248);
        }
        return new String(c);
    }
    ///
    /// 轉半形的函數(DBC case)
    ///
    ///任一字元串
    ///半形字元串
    ///
    ///全形空格為12288，半形空格為32
    ///其他字元半形(33-126)與全形(65281-65374)的對應關係是：均相差65248
    ///
    public static String toDBC(this string input)
    {
        char[] c = input.ToCharArray();
        for (int i = 0; i < c.Length; i++)
        {
            if (c[i] == 12288)
            {
                c[i] = (char)32;
                continue;
            }
            if (c[i] > 65280 && c[i] < 65375)
                c[i] = (char)(c[i] - 65248);
        }
        return new String(c);
    }

    public static bool isMail(this string input)
    {
        try
        {
            MailAddress m = new MailAddress(input);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
    public static string format(this string temp, params string[] args) => string.Format(temp, args);
    public static string formatNewLineToHtml(this string input)
    {
        return TextUtil.formatNewLineToHtml(input);
    }
    public static string cleanXss(this string input)
    {
        return TextUtil.cleanHtmlFragmentXss(input);
    }

    public static void creatDir(this string dirPath)
    {
        if (!Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
    }
    public static string toFilePath(this string path)
    {
        return WebUtil.getFileDir() + path.Replace('/', '\\');
    }
    public static string toServerPath(this string path)
    {
        if (path.StartsWith("~/"))
            return HttpContext.Current.Server.MapPath(path);
        else if (path.StartsWith("/"))
            return HttpContext.Current.Server.MapPath("~" + path);
        else
            return HttpContext.Current.Server.MapPath("~/" + path);
    }
    public static string toLinkUrl(this string path)
    {
        if (path.StartsWith("/"))
            return WebUtil.getWebURL() + path;
        else
            return WebUtil.getWebURL() + "/" + path;
    }
    public static int toInt(this object input)
    {
        try
        {
            return Convert.ToInt32(input);
        }
        catch (Exception)
        {
            return 0;
        }
    }
    public static E toEnum<E>(this string input, bool ignoreCase = false)
    {
        return (E)Enum.Parse(typeof(E), input, ignoreCase);
    }
    #endregion
    #region StringBuilder
    public static StringBuilder removeLastChar(this StringBuilder input)
    {
        if (input.Length > 0)
            input.Remove(input.Length - 1, 1);
        return input;
    }
    #endregion
    #region Enum extension
    public static E toEnum<E>(this int input)
    {
        return (E)Enum.ToObject(typeof(E), input);
    }
    public static bool containEnumValue<E>(this int input) => Enum.IsDefined(typeof(E), input);
    public static int toInt(this Enum input)
    {
        return Convert.ToInt32(input);
    }

    public static string getDescription(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }
    #endregion

    #region Json Convert
    //public static string toJsonString<T>(this IList<T> list)
    //{
    //    return JsonConvert.SerializeObject(list);
    //}
    public static string toJsonString(this object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
    public static T toObj<T>(this object input)
    {
        try
        {
            //目前先這樣 之後有機會再改看看
            return input.toJsonString().toObj<T>();
        }
        catch (Exception) { }
        return default(T);
    }
    public static T toObj<T>(this string input)
    {
        try
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
        catch(Exception) { }
        return default(T);
    }
    public static List<T> toList<T>(this string input)
    {
        try
        {
            return JsonConvert.DeserializeObject<List<T>>(input);
        }
        catch (Exception) { }
        return new List<T>();
    }

    public static object removeObjAttr(this object data, params string[] attrs)
    {
        var obj = JObject.FromObject(data);
        foreach (var key in attrs)
            obj.Remove(key);
        return obj;
    }
    public static List<JObject> removeListAttr<T>(this IEnumerable<T> list, params string[] attrs)
    {
        var cleanList = new List<JObject>();
        foreach (var data in list)
        {
            var obj = JObject.FromObject(data);
            foreach (var key in attrs)
                obj.Remove(key);

            cleanList.Add(obj);
        }
        return cleanList;
    }
    #endregion

    #region object property extension
    public static T copyTo<T>(this object input)=> input.copyTo(Activator.CreateInstance<T>());
    public static T copyTo<T>(this object input,T instance)
    {
        //Log.d($"Type->{input.GetType().Name}");
        foreach(var prop in input.GetType().GetProperties())
        {
            try
            {
                var name = prop.Name;
                var value = prop.GetValue(input);
                //Log.d($"prop name->{name}  value->{value}");
                instance.setObjValue(name, value);
            }
            catch
            {
                //有不能設定的屬性 就跳過 繼續
            }
        }

        return instance;
    }
    public static void setObjValue(this object input,string name,object value)
    {
        var type = input.GetType();
        if (type.GetProperty(name) != null)
            type.GetProperty(name).SetValue(input, value);
    }
    public static T getObjValue<T>(this object obj, string propertyName)
    {
        return (T)obj.GetType().GetProperty(propertyName).GetValue(obj, null);
    }
    public static IEnumerable<PropertyInfo> getAttrProperty<T>(this object obj)where T : Attribute
    {
        return (from p in obj.GetType().GetProperties()
                where p.isDefinedAttr<T>()
                select p);
    }
    public static T getPropertyAttr<T>(this object obj, string propertyName) where T : Attribute
    {
        //以後再看看要不要開起繼承的屬性
        return (T)obj.GetType().GetProperty(propertyName).GetCustomAttributes(typeof(T), true).FirstOrDefault();
    }
    public static bool isDefinedAttr<T>(this PropertyInfo info)where T : Attribute
    {
        return info.IsDefined(typeof(T), true);
    }
    public static bool isDefinedAttr<T>(this Type type) where T : Attribute
    {
        return type.IsDefined(typeof(T), true);
    }
    public static T getAttribute<T>(this Type t, bool inherit=true) where T : Attribute
    {
        return t.GetCustomAttribute<T>(inherit);
    }
    public static T getAttribute<T>(this PropertyInfo info) where T : Attribute
    {
        //以後再看看要不要開起繼承的屬性
        return (T)info.GetCustomAttributes(typeof(T), true).FirstOrDefault();
    }
    public static void notNullOrEmpty<T>(this T obj,Action<T> back)
    {
        if (!obj.isNullOrEmpty())
            back(obj);
    }
    public static bool isNullOrEmpty(this DateTime t)
    {
        if (t == default(DateTime))
            return true;
        if (t == DateTime.MinValue)
            return true;

        return false;
    }
    public static bool isNullOrEmpty(this object obj)
    {
        try
        {
            var type = obj.GetType();
            if (type == typeof(string) || type == typeof(String))
            {
                return string.IsNullOrEmpty(obj.ToString());
            }
            return obj == null;
        }
        catch (Exception )
        {
        }
        return true;
    }
    #endregion
    #region IEnumable
    public static IQueryable<T> Like<T>(this IQueryable<T> source, string propertyName, string keyword)
    {
        var type = typeof(T);
        var property = type.GetProperty(propertyName);
        var parameter = Expression.Parameter(type, "p");
        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var constant = Expression.Constant("%" + keyword + "%");
        MethodCallExpression methodExp = Expression.Call(null, typeof(SqlMethods).GetMethod("Like", new Type[] { typeof(string), typeof(string) }), propertyAccess, constant);
        Expression<Func<T, bool>> lambda = Expression.Lambda<Func<T, bool>>(methodExp, parameter);
        return source.Where(lambda);
    }
    public static List<T> toSafeList<T>(this IEnumerable<T> source)
    {
        var list = source.ToList();
        if (list == null)
            list = new List<T>();
        return list;
    }
    #endregion
    #region List extension
    public static void Foreach<T>(this IEnumerable<T> list,Action<T> back)
    {
        foreach (var v in list)
            back(v);
    }
    //檢查List裡面是否存在 if not exsist return ture
    public static bool notAny<T>(this IEnumerable<T> list,Func<T,bool> back) 
    {
        return !list.Any(item=> { return back(item); });
    }
    public static bool hasData<T>(this IEnumerable<T> list)
    {
        return list.Count() > 0;
    }
    //檢查List裡面是否存在 if exsist return ture
    //public static bool any<T>(this IEnumerable<T> list,Func<T,bool> back)
    //{
    //    return list.Any(item => { return back(item); });
    //}
    #endregion
    #region Map extension
    public static void Foreach<K,V>(this ICollection<KeyValuePair<K, V>> input,Action<K,V> back)
    {
        foreach (var pair in input)
            back(pair.Key, pair.Value);
    }
    public static string get(this Dictionary<string, string> dic,string key)
    {
        if (dic.ContainsKey(key))
            return dic[key];
        return "";
    }
    #endregion

    #region IFormDataControl
    public static bool formDataContain(this IFormDataControl c, string key)
    {
        if (HttpContext.Current.Request.Form.AllKeys.Contains(key))
        {
            return !HttpContext.Current.Request.Form[key].isNullOrEmpty();
        }
        return false;
    }
    public static bool formFileContain(this IFormDataControl c,string key)
    {
        if (HttpContext.Current.Request.Files.AllKeys.Contains(key))
        {
            return HttpContext.Current.Request.Files[key] != null;
        }
        return false;
    }
    public static T getPostObj<T>(this IFormDataControl c, string flag)
    {
        return HttpContext.Current.Request.Form[flag].toObj<T>();
        //return ConvertToObj<T>(HttpContext.Current.Request.Form[flag]);
    }
    public static HttpPostedFile getPostFile(this IFormDataControl c, string flag)
    {
        if (c.formFileContain(flag))
            return HttpContext.Current.Request.Files[flag];
        return null;
    }
    #endregion

    #region Type extension

    /// <summary>
    /// 檢查父類別是否有繼承該介面
    /// </summary>
    /// <typeparam name="CHILD"></typeparam>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static bool hasInterface<CHILD>(this object parent)
    {
        return typeof(CHILD).IsAssignableFrom(parent.GetType());
    }
    #endregion
}