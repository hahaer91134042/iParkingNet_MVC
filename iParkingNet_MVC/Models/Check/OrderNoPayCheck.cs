using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderNoPayCheck 的摘要描述
/// </summary>
public class OrderNoPayCheck : ICheckValid
{
    private IEnumerable<EkiOrder> orders;

    public OrderNoPayCheck(IEnumerable<EkiOrder> list)
    {
        orders = list;
    }
    public void validCheck()
    {
        //找出已用過但是未結帳的單
        var nopay = (from order in orders
                     where order.isNoPayOrder()
                     select order).Count();
        //之後要開啟
        if (nopay > 0)
            throw new OrderNoPayException();
    }
}