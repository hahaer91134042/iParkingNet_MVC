using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Verify 的摘要描述
/// </summary>
[DbTableSet("Verify")]
public class Verify : BaseDbDAO
{
    [DbRowKey("Type")]
    public int Type { get => typeEnum.toInt(); set { typeEnum = value.toEnum<VerifyType>(); } }
    [DbRowKey("ItemId")]
    public int ItemId { get; set; } = 0;
    [DbRowKey("Status",DbAction.Update)]
    public int Status { get => statusEnum.toInt(); set { statusEnum = value.toEnum<VerifyStatus>(); } }
    [DbRowKey("Text", DbAction.Update)]
    public string Text { get; set; } = "";
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.UpdateOnly, true)]
    public DateTime uDate { get; set; }

    public VerifyType typeEnum { get; set; } = VerifyType.Order;
    public VerifyStatus statusEnum { get; set; } = VerifyStatus.Processing;

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);

    public override bool Update() => EkiSql.ppyp.update(this);
}