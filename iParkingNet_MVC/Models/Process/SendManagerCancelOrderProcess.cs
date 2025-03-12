using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderCancelProcess 的摘要描述
/// </summary>
public class SendManagerCancelOrderProcess : BaseProcess
{
    private List<EkiOrder> orderList;
    private DateTime now;
    public SendManagerCancelOrderProcess(List<EkiOrder> list,DateTime from)
    {
        orderList = list;
        now = from;
    }

    public override void run()
    {
        var orderMembers = (from m in EkiSql.ppyp.table<Member>()
                            where orderList.Any(o => o.MemberId == m.Id)
                            select m);
        var msgPair = new Dictionary<Member, IBroadCastMsg>();

        orderList.ForEach(order =>
        {
            var orderMember = orderMembers.First(m => order.MemberId == m.Id);
            //加入折扣碼
            //var discountAmt = order.calCompensation(now);
            var discountResponse = order.calCompensation(now).Let(amt =>
            {
                if (amt > 0)
                {
                    var discount = new MemberDiscount()
                    {
                        MemberId = orderMember.Id,
                        Code = SerialNumUtil.DiscountSerialNum(),
                        Amt = amt,
                        IsRange = true,
                        StartTime = now,
                        EndTime = now.AddDays(ApiConfig.Discount.MaxValidDay)
                    };
                    discount.Insert();
                    return discount.convertToResponse();
                }
                return null;
            });

            //加入發送的訊息
            msgPair.Add(orderMember, new ManagerOrderCancelContent()
            {
                Order = order.convertToResponse(),
                Discount = discountResponse
            });
        });
        msgPair.Foreach((member, msg) => msg.sendTo(member) );
    }
}