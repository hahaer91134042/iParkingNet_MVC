using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

/// <summary>
/// ArgueManager 的摘要描述
/// </summary>
public class ArgueManager:BaseManager
{
    public Member member;
    public static ArgueManager from(JwtAuthObject obj)
    {
        return new ArgueManager(obj.getMember());
    }
    ArgueManager(Member m)
    {
        member = m;
    }

    public bool addArgue(ArgueRequest request,Func<EkiPostImg> getImg)
    {
        var order = (from o in GetTable<EkiOrder>()                    
                     where o.SerialNumber == request.serial
                     where o.StatusEnum == OrderStatus.CheckOut

                     join loc in GetTable<Location>() on o.LocationId equals loc.Id
                     //假如這筆申訴是地主發出的=>檢查該地點是不是同一個member
                     where request.source.toEnum<ArgueSource>()==ArgueSource.Manager?
                     loc.MemberId==member.Id:o.MemberId==member.Id
                     
                     select o).FirstOrDefault();
        if (order.isNullOrEmpty())
            throw new ArgumentException();

        if ((from a in GetTable<Argue>()
             where a.OrderId == order.Id
             where a.MemberId == member.Id
             where a.Type == request.type
             select a).Count() > 0)
            throw new AddErrorException();
        try
        {
            
            var argue = new Argue
            {
                OrderId = order.Id,
                LocationId = order.LocationId,
                MemberId = member.Id,
                Text = request.text,
                Type = request.type,
                Source = request.source,
                Lat = request.lat,
                Lng = request.lng
            };
            getImg().notNull(img =>
            {
                img.saveBitmapWith(member);
                argue.Img = img.imgName();
            });

            
            argue.Insert();

            return true;
        }
        catch (Exception)
        {
        } 
        return false;
    }

}