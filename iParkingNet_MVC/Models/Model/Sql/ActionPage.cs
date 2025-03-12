using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ActionPage 的摘要描述
/// </summary>
[DbTableSet("ActionPage")]
public class ActionPage : BaseDbDAO,IConvertResponse<ResponseContent.ActionPage>
{
    [DbRowKey("ActionId")]
    public int ActionId { get; set; }
    [DbRowKey("Html",DbAction.Update)]
    public string Html { get; set; }
    [DbRowKey("Url",DbAction.Update)]
    public string Url { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }

    public ResponseContent.ActionPage convertToResponse() =>
        new ResponseContent.ActionPage
        {
            Html = Html,
            Url = Url
        };

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);

    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);
}