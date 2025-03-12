using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ServerNotify 的摘要描述
/// </summary>
[DbTableSet("ServerNotify")]
public class ServerNotify : BaseDbDAO,IConvertResponse<NotifyResponse>
{
    [DbRowKey("Title",DbAction.Update)]
    public string Title { get; set; }
    [DbRowKey("Msg",DbAction.Update)]
    public string Msg { get; set; }
    [DbRowKey("Html",DbAction.Update)]
    public string Html { get; set; }
    [DbRowKey("StartTime", RowAttribute.Time, DbAction.Update)]
    public DateTime StartTime { get; set; }
    [DbRowKey("EndTime", RowAttribute.Time, DbAction.Update)]
    public DateTime EndTime { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }

    public NotifyResponse convertToResponse() =>
        new NotifyResponse
        {
            Type=NotifyType.Server,
            Content=new ResponseContent.Notify
            {
                Title=Title,
                Msg=Msg,
                Html=Html
            }
        };

    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }

    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }

}