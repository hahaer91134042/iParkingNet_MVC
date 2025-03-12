using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EkiAction 的摘要描述
/// </summary>
[DbTableSet("Action")]
public class EkiAction : BaseDbDAO,IConvertResponse<ActionResponse>
{
    [DbRowKey("Code",DbAction.Update,false)]
    public string Code { get; set; }
    [DbRowKey("Name",DbAction.Update)]
    public string Name { get; set; }
    [DbRowKey("TimeLimit", DbAction.Update)]
    public bool TimeLimit { get; set; } = false;
    [DbRowKey("StartTime",RowAttribute.Time,DbAction.Update)]
    public DateTime StartTime { get; set; }
    [DbRowKey("EndTime", RowAttribute.Time, DbAction.Update)]
    public DateTime EndTime { get; set; }
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; } = true;
    [DbRowKey("Type", DbAction.Update)]
    public int Type { get { return actionType.toInt(); } set { actionType = value.toEnum<ActionType>(); } }
    [DbRowKey("Detail",DbAction.Update)]
    public string Detail { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.UpdateOnly, true)]
    public DateTime uDate { get; set; }

    public ActionType actionType = ActionType.Discount;
    public object typeDetail {
        get 
        {
            switch (actionType)
            {
                case ActionType.Discount:
                    return Detail.toObj<Discount>();
            }
            return new { };
        }
        set
        {
            Detail = value.toJsonString();
        }
    }

    public T getTypeDetail<T>() where T:IActionDetail =>(T)typeDetail;
    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);
    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);

    public ActionResponse convertToResponse() => new ActionResponse
    {
        Type=Type,
        Detail=typeDetail
    };

    public NotifyResponse getNotifyResponse() 
    {
        var page = (from p in EkiSql.ppyp.table<ActionPage>()
                    where p.ActionId == Id
                    select p).FirstOrDefault();
        return new NotifyResponse
        {
            Type = NotifyType.Action,
            Content = new ResponseContent.Action
            {
                Name=Name,
                Page=page?.convertToResponse()
            }            
        };
    }

        

    //標記用
    public interface IActionDetail { }
    //要新增不同活動類型的的類別在這邊
    public class Discount:IActionDetail
    {
        public double Number { get; set; } = 0d;
        public double Ratio { get; set; } = 0d;
    }
}