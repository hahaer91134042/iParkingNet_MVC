using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


/// <summary>
/// SerialUtil 的摘要描述
/// </summary>
public class SerialNumUtil
{

    public static string DiscountSerialNum()
    {
        var rule = new DiscountCodeRule();
        var table = EkiSql.ppyp.table<MemberDiscount>();
        string num;
        do
        {
            //ranStr = RandomUtil.GetRandomString(ApiConfig.Discount.CodeLength, RandomString.All) +
            //       RandomUtil.GetRandomString(ApiConfig.Discount.CodeFullLength - ApiConfig.Discount.CodeLength, RandomString.All);

            num = RandomUtil.GetRandomString(ApiConfig.Discount.CodeLength, RandomString.All) +
                    RandomUtil.GetRandomString(ApiConfig.Discount.CodeFullLength - ApiConfig.Discount.CodeLength, RandomString.All);

        } while (!rule.isInRule(num) || table.Any(d => d.Code.Equals(num)));
        return num;
    }
    public static bool IsDiscountSerialNum(string input) => new DiscountCodeRule().isInRule(input);

    //private static DateTime Now = DateTime.Now;
    public static string OrderTimeSerialNum(DateTime time)
    {   //這樣沒有特殊碼才能給支付公司使用
        return $"{ApiConfig.ParkingOrderSerHead}{TimeUtil.toStamp(time)}{RandomUtil.GetRandonNumStr(4)}";
    }
    public static string EkiOrderSerialNum()
    {   //這樣沒有特殊碼才能給支付公司使用
        return $"{ApiConfig.EkiOrderSerHead}{TimeUtil.toStamp(DateTime.Now)}{RandomUtil.GetRandonNumStr(4)}";
    }
    //目前棄用
    //public static string OrderSerialNum()
    //{
    //    var keyNum = Convert.ToDecimal(TextUtil.StrASC(ApiConfig.SerialKey));
    //    int result;
    //    string serialNum = "";

    //    do
    //    {
    //        try
    //        {
    //            var body = RanSerialNum2();
    //            var digest1 = Convert.ToDecimal(body.Substring(0, 4));
    //            var digest2 = Convert.ToDecimal(body.Substring(4, 4));
    //            result = Convert.ToInt32((digest1 * digest2) / keyNum) % 10;
    //            if (result == 1)
    //                serialNum = $"{ApiConfig.ParkingOrderSerHead}-{body}";
    //        }
    //        catch (Exception)
    //        {
    //            result = -1;
    //        }
    //    } while (result!=1);

    //    return serialNum;
    //}

    public static string LocationSerialNum()
    {
        var keyNum = Convert.ToDecimal(TextUtil.StrASC(ApiConfig.SerialKey));
        int result;
        string serialNum = "";
        do
        {
            try
            {
                var part1 = RandomUtil.GetSerialNumberChar();
                var part2 = RanSerialNum();

                var part1Num = TextUtil.IntASC(part1);
                var digest1 = Convert.ToDecimal(part2.Substring(0, 3));
                var digest2 = Convert.ToDecimal(part2.Substring(3, 3));

                result = Convert.ToInt32((part1Num * digest1 * digest2) / keyNum) % 10;
                if (result == 0)
                    serialNum = $"{part1}-{part2}";
            }
            catch (Exception)
            {
                result = -1;
            }
        } while (result != 0);

        return serialNum;
    }
    public static bool isLocationSerialNum(string serialNum)
    {
        try
        {
            var keyNum = TextUtil.IntASC(ApiConfig.SerialKey);
            var arr = serialNum.Split('-');
            var part1Num = TextUtil.IntASC(arr[0]);
            var digest1 = Convert.ToDecimal(arr[1].Substring(0, 3));
            var digest2 = Convert.ToDecimal(arr[1].Substring(3, 3));
            return (Convert.ToInt32((part1Num * digest1 * digest2) / keyNum) % 10) == 0;
        }
        catch (Exception)
        {
            return false;
        }
    }
    private static string RanSerialNum2()
    {
        
        var v1 = ran.Next(1000, 1999).ToString();
        var v2 = ran.Next(1000, 9999).ToString();
        var v3 = ran.Next(0000, 9999).ToString().PadLeft(4, '0');
        return $"{v1}{v2}{v3}";
    }
    //避免static method裡面都使用同一個seed
    private static Random ran = new Random();
    private static string RanSerialNum()
    {

        var v1 = ran.Next(100, 999).ToString();
        var v2 = ran.Next(100, 999).ToString();
        var v3 = ran.Next(000, 999).ToString().PadLeft(3, '0');
        return $"{v1}{v2}{v3}";
    }
}