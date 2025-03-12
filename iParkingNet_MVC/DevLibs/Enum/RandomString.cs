using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// RandomTemplete 的摘要描述
/// </summary>
public class RandomString
{
    public static RandomString UpperString = new RandomString("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
    public static RandomString LowerString = new RandomString("abcdefghijklmnopqrstuvwxyz");
    public static RandomString NumberString = new RandomString("0123456789");
    public static RandomString All = new RandomString("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");

    public string templete;
    public RandomString(string v)
    {
        this.templete = v;
    }
}