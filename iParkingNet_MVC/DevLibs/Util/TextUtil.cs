using Microsoft.Security.Application;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// TextUtil 的摘要描述
/// </summary>
#region ---TextUtil--- 使用AntiXSS 4.3
public class TextUtil
{
    public static string cleanHtmlDocXss(string input)
    {
        return Sanitizer.GetSafeHtml(input);
    }
    public static string cleanHtmlFragmentXss(string input)
    {
        return Sanitizer.GetSafeHtmlFragment(input);
    }
    //public static string cleanHtmlDocXss(string input)
    //{
    //    return AntiXss.GetSafeHtml(input);
    //}
    //public static string cleanHtmlFragmentXss(string input)
    //{
    //    return AntiXss.GetSafeHtmlFragment(input);
    //}

    public static char Chr(int Num)
    {
        return Convert.ToChar(Num);
    }
    public static string StrASC(string str)
    {
        var strBuilder = new StringBuilder();
        foreach (var c in str.ToCharArray())
        {
            strBuilder.Append(Convert.ToString(ASC(c)));
        }
        return strBuilder.ToString();
    }
    public static decimal IntASC(string str)
    {
        return Convert.ToDecimal(StrASC(str));
    }
    public static int ASC(char C)
    {
        return Convert.ToInt32(C);
    }
    /// <summary>
    /// URL编码 假如php javascript .net有問題的話使用
    /// </summary>
    /// <param name="value">The value to Url encode</param>
    /// <returns>Returns a Url encoded string</returns>
    public static string UrlEncode(string value)
    {
        StringBuilder result = new StringBuilder();

        string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        foreach (char symbol in value)
        {
            if (unreservedChars.IndexOf(symbol) != -1)
            {
                result.Append(symbol);
            }
            else
            {
                result.Append('%' + String.Format("{0:X2}", (int)symbol));
            }
        }

        return result.ToString();
    }

    public static string getSaftUrl(string input)
    {
        return HttpUtility.UrlEncode(cleanHtmlFragmentXss(input));
    }

    public static string removeEscape(string input)
    {
        return input.Replace("\\", "");
    }
    public static string formatNewLineToHtml(string input)
    {
        return replaceTextNewLineTo(input, "<br/>");
    }
    public static string formatSpaceToHtml(string input)
    {
        return input.Replace(" ", "&nbsp;");
    }
    public static string formatSpaceToHtml2(string input)
    {
        return input.Replace(" ", "&ensp;");
    }
    public static string formatSpaceToHtml4(string input)
    {
        return input.Replace(" ", "&emsp;");
    }
    public static string replaceTextNewLineTo(string input, string replaceStr)
    {
        return input.Replace("\r\n", replaceStr);//換行這個regix是固定的.....
    }
    public static string textToNewLine(string input, string oriStr)
    {
        return input.Replace(oriStr, "\r\n");
    }

    public static bool checkPwdVaild(string pwd)
    {
        if (!string.IsNullOrEmpty(pwd))
        {
            if (checkUpperCase(pwd) && checkLowerCase(pwd) && checkNumber(pwd) && checkTextLenght(pwd, 8))
                return true;
        }
        return false;
    }

    public static bool checkTextLenght(string input, int min = 0)
    {
        return input.Length >= min;
    }

    public static bool checkUpperCase(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, "[A-Z]");
    }

    public static bool checkLowerCase(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, "[a-z]");
    }

    public static bool checkNumber(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, "[0-9]");
    }

    public static bool isNumber(string input)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[0-9]*$");
    }

    //https://docs.microsoft.com/zh-tw/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
    public static bool checkEMailVaild(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                  RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                var domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException e)
        {
            return false;
        }
        catch (ArgumentException e)
        {
            return false;
        }

        try
        {
            return Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
    //https://stackoverflow.com/questions/19087183/regex-for-12hr-and-24hr-time-format
    //可對12hr or 24hr 時間 hh:mm am OR hh:mm OR hh:mm:ss
    public static bool checkTimeFormat(string input)
    {
        return Regex.IsMatch(input, @"^(?:(?:0?[0-9]|1[0-2]):[0-5][0-9] [ap]m|(?:[01][0-9]|2[0-4]):[0-5][0-9]|(?:[01][0-9]|2[0-4]):[0-5][0-9]:[0-5][0-9])$", RegexOptions.IgnoreCase);
    }
}
#endregion