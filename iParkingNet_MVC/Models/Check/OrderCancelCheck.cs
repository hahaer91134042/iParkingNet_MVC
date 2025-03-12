using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderCancelCheck 的摘要描述
/// </summary>
public class OrderCancelCheck : ICheckValid
{
    private int delCount;
    private List<EkiOrder> delOrder;

    public OrderCancelCheck( int delCount, List<EkiOrder> delOrder)
    {
        this.delCount = delCount;
        this.delOrder = delOrder;
    }

    public void validCheck()
    {
        if (delCount >= ApiConfig.OrderCancelLimit)
            throw new OutOfLimitException();

        if ((delCount + delOrder.Count) > ApiConfig.OrderCancelLimit)
            throw new OutOfLimitException();
    }
}