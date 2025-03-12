using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderSameTimeCheck 的摘要描述
/// </summary>
public class OrderSameTimeCheck : ICheckValid
{
    private IEnumerable<EkiOrder> orders;
    private DbObjList<ReservedTime> reservaList;//使用者要預約的時間

    public OrderSameTimeCheck( IEnumerable<EkiOrder> list, DbObjList<ReservedTime> rList)
    {        
        orders =list;
        reservaList = rList;
    }

    public void validCheck()
    {
        var forbiCheck = new ForbiddenCheck((from o in orders
                                            where o.ReservaTime.IsCancel==false
                                             //找出該筆訂單的預約日期跟使用者要預約的日期清單裡面是否有一樣的
                                             where (from reserva in reservaList
                                                    where reserva.getStartTime().Date==o.ReservaTime.getStartTime().Date
                                                    select reserva).Count()>0
                                            select o.ReservaTime).toSafeList());
        //要檢查該使用的預約時段是否有同樣的時段 有的話就不能預約
        var count = (from time in reservaList
                 where forbiCheck.IsForbidden(time)
                 select time).Count();
        if (count > 0)
            throw new TimeOverlapException();
    }
}