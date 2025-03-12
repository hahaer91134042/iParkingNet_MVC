using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// PriceMethodObj 的摘要描述
/// </summary>
public class PriceMethodSet
{
    public static PriceMethodSet Fail = new PriceMethodSet(-1, 0);
    public static PriceMethodSet Per30Min=new PriceMethodSet(PriceMethod.Per30Min.toInt(),30);
   
    
    public int Value;
    public int Min;

    PriceMethodSet(int v, int min)
    {
        Value = v;
        Min = min;
    }

    public static PriceMethodSet parse(int v)
    {
        switch (v.toEnum<PriceMethod>())
        {
            case PriceMethod.Per30Min:
                return Per30Min;
            default:
                return Fail;
        }
    }
}