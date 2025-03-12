using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderPayRecord 的摘要描述
/// 這個表個主要是因應藍新的訂單編號只能使用一次 
/// 所以要改用這邊的EkiOrderSerial 來當藍新要使用的訂單編號
/// (PPYP)的訂單編號不用更改
/// </summary>
[DbTableSet("OrderPayRecord")]
public class OrderPayRecord : BaseDbDAO
{
    [DbRowKey("OrderId",false)]
    public int OrderId { get; set; }
    [DbRowKey("EkiOrderSerial", false)]
    public string EkiOrderSerial { get; set; }
    [DbRowKey("UniqueID", RowAttribute.Guid, true)]
    public Guid UniqueID { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("Action",false)]
    public string Action { get => action.ToString(); set => action = value.toEnum<ActionOption>(); }

    public ActionOption action = ActionOption.NewebPay;
    public static OrderPayRecord NewebPay(int orderId) => new OrderPayRecord()
    {
        OrderId=orderId,
        EkiOrderSerial=SerialNumUtil.EkiOrderSerialNum()
    };
    public static OrderPayRecord LinePay(int orderId) => new OrderPayRecord()
    {
        OrderId = orderId,
        EkiOrderSerial = SerialNumUtil.EkiOrderSerialNum(),
        action=ActionOption.LinePay
    };

    public override bool CreatById(int id) => EkiSql.ppyp.loadDataById(id, this);
    public override int Insert(bool isReturnId = false) => EkiSql.ppyp.insert(this, isReturnId);

    public enum ActionOption
    {
        NewebPay,
        LinePay
    }
}