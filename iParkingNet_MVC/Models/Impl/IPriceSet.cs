using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// IPriceSet 的摘要描述
/// </summary>
public interface IPriceSet<T>:ICurrencySet
{
    T price();
    PriceMethodSet methodSet();    
}