using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

/// <summary>
/// DiscountCodeRule 的摘要描述
/// 處理規則:收到的字串(7主要碼轉ascII+1隨機去掉)的數字編碼在跟secret的asc數字相除%10 ==3
/// </summary>
public class DiscountCodeRule : IRuleCheck<string>
{
    private decimal secret = TextUtil.IntASC(ApiConfig.Discount.Secret);

    public bool isInRule(string factor)
    {
        try
        {
            if (factor.Length != ApiConfig.Discount.CodeFullLength)
                return false;
            var sub = factor.Substring(0, ApiConfig.Discount.CodeLength);
            
            var ans = ((TextUtil.IntASC(sub) / secret) % 10).toInt();

            // this.saveLog($"factor->{factor} sub->{sub} ans->{ans}");
            return ans == 3;
             //!table.Any(d => d.Code.Equals(factor))
            //((TextUtil.IntASC(ranStr.Substring(0, ApiConfig.Discount.CodeLength)) / TextUtil.IntASC(secret)) % 10).toInt();
        }
        catch (Exception)
        {
            return false;
        }
    }
}