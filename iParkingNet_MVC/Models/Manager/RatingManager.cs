using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

/// <summary>
/// RatingManager 的摘要描述
/// </summary>
public class RatingManager:BaseManager
{
    public Member member;
    public static RatingManager from(JwtAuthObject auth)
    {
        return new RatingManager(auth.getMember());
    }
    RatingManager(Member m)
    {
        member = m;
    }

    public bool addLocationRating(RatingRequest request)
    {
        //這邊的member是車主
        var order = (from o in GetTable<EkiOrder>()
                     where o.MemberId==member.Id
                     where o.SerialNumber == request.serial
                     //where o.StatusEnum==OrderStatus.CheckOut
                     select o).FirstOrDefault();
        if (order == null) throw new ArgumentNullException();

        //已經評過分了
        if ((from r in GetTable<MemberRating>()
             where r.OrderId == order.Id && r.MemberId == member.Id
             select r).Count() > 0)
            throw new AddErrorException();

        try
        {
            var memberRating = new MemberRating
            {
                OrderId = order.Id,
                LocationId = order.LocationId,
                MemberId = member.Id,
                Star = request.star,
                Text = request.text
            };
            memberRating.Insert();

            return true;
        }
        catch (Exception)
        {            
        }  
        return false;
    }

    public bool addUserRating(RatingRequest request)
    {
        //這邊的member是地主
        var order = (from o in GetTable<EkiOrder>()
                     join l in GetTable<Location>() on o.LocationId equals l.Id
                     where l.MemberId == member.Id
                     where o.SerialNumber == request.serial
                     //where o.StatusEnum==OrderStatus.CheckOut
                     select o).FirstOrDefault();
        if (order == null) throw new ArgumentNullException();

        //已經評過分了
        if ((from r in GetTable<ManagerRating>()
             where r.OrderId == order.Id && r.MemberId == member.Id
             select r).Count() > 0)
            throw new AddErrorException();

        try
        {
            var rating = new ManagerRating
            {
                OrderId = order.Id,
                UserMemberId = order.MemberId,//該訂單的車主
                MemberId = member.Id,
                Star = request.star,
                Text = request.text
            };
            rating.Insert();

            return true;
        }
        catch (Exception) { }
        return false;
    }
}