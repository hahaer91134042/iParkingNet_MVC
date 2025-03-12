using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderResponse 的摘要描述
/// </summary>
public class EkiOrderResponse : ApiAbstractModel
{
    public string SerialNumber { get; set; }
    public decimal Cost { get; set; }
    public int Unit { get; set; }
    public double LocPrice { get; set; }
    public string LocSerial { get; set; }
    //public decimal Tax { get; set; }
    public decimal HandlingFee { get; set; }
    public int Status { get; set; }
    public string CarNum { get; set; }
    public string CreatTime { get; set; }//產生時間
    public string CheckOutUrl { get; set; }
    public CpResponse Cp { get; set; }//表示該訂單所對應的充電樁訊息
    public bool Rating { get; set; } = false;
    public bool Argue { get; set; } = false;
    public MemberResponse Member { get; set; }
    public ReservaTimeResponse ReservaTime { get; set; }
    public AddressResponseModel Address { get; set; }
    public CheckoutResponse Checkout { get; set; }
    public InvoiceResponse Invoice { get; set; }

    public EkiOrderResponse setMember(Member m)
    {
        Member = m.convertToResponse();
        return this;
    }

    public EkiOrderResponse setLoc(Location loc)
    {
        //LocPrice = loc.ReservaConfig.Price.toDouble();
        LocSerial = loc.SerNum;
        Cp = loc.Cp != null ? loc.Cp.convertToResponse() : null;
        return this;
    }
    public EkiOrderResponse setRating(bool rating)
    {
        Rating = rating;
        return this;
    }
    public EkiOrderResponse setArgue(bool argue)
    {
        Argue = argue;
        return this;
    }
}