using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// RandomUtil 的摘要描述
/// </summary>
public class RandomUtil
{
    private static Random ran = new Random();
    public static string GetRandonNumStr(int length = 3)
    {
        var min = new StringBuilder();
        var max = new StringBuilder();
        for (var i = 0; i < length; i++)
        {
            min.Append("0");
            max.Append("9");
        }
        
        return ran.Next(Convert.ToInt32(min.ToString()), Convert.ToInt32(max.ToString())).ToString().PadLeft(length, '0');
    }

    public static string GetSerialNumberChar(int length=3)
    {
        return GetRandomString(length, RandomString.UpperString);
    }

    public static string GetRandomString(int length,RandomString randomString)
    {
        //var str = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        //var ran = new Random();
        var builder = new StringBuilder();
        var tempLength = randomString.templete.Length;
        for (var i = 0; i < length; i++)
        {
            builder.Append(randomString.templete[ran.Next(0, tempLength)]);
        }
        return builder.ToString();
    }
}