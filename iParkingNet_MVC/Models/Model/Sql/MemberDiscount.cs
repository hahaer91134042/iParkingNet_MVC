using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Discount 的摘要描述
/// </summary>
[DbTableSet("MemberDiscount")]
public class MemberDiscount : BaseDbDAO,IConvertResponse<DiscountResponse>
{
    [DbRowKey("MemberId", false)]
    public int MemberId { get; set; }
    [DbRowKey("Code",false)]
    public string Code { get; set; }
    [DbRowKey("Amt")]
    public double Amt { get; set; } = 0;
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; } = true;
    [DbRowKey("IsRange", DbAction.Update)]
    public bool IsRange { get; set; }
    [DbRowKey("StartTime", RowAttribute.Time, DbAction.Update, true)]
    public DateTime StartTime { get; set; }//雖然有開始時間但是使用上只要沒超過EndTime都可以用
    [DbRowKey("EndTime", RowAttribute.Time, DbAction.Update, true)]
    public DateTime EndTime { get; set; }

    public DiscountResponse convertToResponse()
    {
        return new DiscountResponse()
        {
            Code=Code,
            Amt=Amt,
            IsRange=IsRange,
            End=EndTime.toString()
        };
    }

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
    public override bool Update()
    {
        return EkiSql.ppyp.update(this);
    }
}