using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ManagerMulct 的摘要描述
/// </summary>
[DbTableSet("ManagerMulct")]
public class ManagerMulct : BaseDbDAO,IConvertResponse<MulctResponse>
{
    [DbRowKey("MemberId", false)]
    public int MemberId { get; set; }//紀錄地主的ID
    [DbRowKey("OrderId", false)]
    public int OrderId { get; set; }//取消的訂單ID
    [DbRowKey("LocationId", false)]
    public int LocationId { get; set; }//產生罰金的地點
    //[DbRowKey("CancelTimeId", false)]
    //public int CancelTimeId { get; set; }
    [DbRowKey("Amount",DbAction.Update)]
    public double Amount { get; set; }//罰金總額
    [DbRowKey("Unit",DbAction.Update)]
    public int Unit { get { return currencyUnit.toInt(); } set { currencyUnit = value.toEnum<CurrencyUnit>(); } }
    [DbRowKey("Paid", DbAction.Update)]
    public bool Paid { get; set; } = false;
    [DbRowKey("Time",RowAttribute.Time,true)]    
    public DateTime Time { get; set; }//紀錄地主哪個時間點產生的罰金(client)
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }//伺服器產生時間

    public CurrencyUnit currencyUnit = CurrencyUnit.TWD;
    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id,this);
    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);

    public MulctResponse convertToResponse() => new MulctResponse()
    {
        Amt=Amount,
        Unit=Unit,
        Paid=Paid,
        Time=Time.toString()
    };
}