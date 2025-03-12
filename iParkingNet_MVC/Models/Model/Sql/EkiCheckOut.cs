using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EkiCheckOut 的摘要描述
/// </summary>
[DbTableSet("CheckOut")]
public class EkiCheckOut : BaseDbDAO,IMapImg,IConvertResponse<CheckoutResponse>
{
    [DbRowKey("OrderId",false)]
    public int OrderId { get; set; }
    /// <summary>
    /// 紀錄車主的ID
    /// </summary>
    [DbRowKey("MemberId")]
    public int MemberId { get; set; }
    [DbRowKey("ActionId")]
    public int ActionId { get; set; }
    [DbRowKey("DiscountId")]
    public int DiscountId { get; set; }
    /// <summary>
    /// CostFix 基本上是負數 用來記錄活動跟使用優惠券所扣除的金額
    /// </summary>
    [DbRowKey("CostFix")]
    public double CostFix { get; set; }
    /// <summary>
    /// Claimant 紀錄車主違停的罰金
    /// </summary>
    [DbRowKey("Claimant")]
    public double Claimant { get; set; } = 0;//車主的總罰金(未扣除要給地主的部分)
    [DbRowKey("Lat")]
    public double Lat { get; set; }
    [DbRowKey("Lng")]
    public double Lng { get; set; }
    [DbRowKey("Img")]
    public string Img { get; set; } = "";
    [DbRowKey("Date",RowAttribute.Time,false)]
    public DateTime Date { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("CreditNum")]
    public string CreditNum { get; set; }
    [DbRowKey("Url")]
    public string Url { get; set; }
    [DbRowKey("Ip", DbAction.Update, true)]
    public string Ip { get; set; }

    public CheckoutResponse convertToResponse()
    {
        return new CheckoutResponse
        {
            Date=Date.toString(),
            CostFix=CostFix,
            Claimant=Claimant,
            Img=this.mapImgUrlWith(DirPath.Order, new Member().Also(m=>m.CreatById(MemberId)))
        };
    }

    //[DbRowKey("UniqueID", RowAttribute.Guid, true)]
    //public string UniqueID { get; set; }

    public void CreatedByOrderId(int id)
    {
        EkiSql.ppyp.loadDataByQueryPair(QueryPair.New().addQuery("OrderId", id), this);
    }
    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }

    public string imgName() => Img;

    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }
}