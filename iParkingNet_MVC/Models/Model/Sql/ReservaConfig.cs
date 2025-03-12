using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// Reservation 的摘要描述
/// </summary>
[DbTableSet("ReservaConfig")]
public class ReservaConfig : BaseDbDAO,IPriceSet<decimal>,IEdit<ReservaConfigRequest>
{
    /// <summary>
    /// 表示該地點有無開啟(預約)
    /// </summary>
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; } = true;
    [DbRowKey("beRepeat", DbAction.Update)]
    public bool beRepeat { get; set; } = true;
    [DbRowKey("Text",DbAction.Update)]
    public string Text { get; set; }
    [DbRowKey("Price",DbAction.Update)]
    public decimal Price { get; set; }
    [DbRowKey("Unit")]
    public int Unit { get { return convertEnumToInt(Currency); } set { Currency = convertIntToEnum<CurrencyUnit>(value); } }
    [DbRowKey("Method")]
    public int Method { get { return PriceMethod.Value; } set { PriceMethod = PriceMethodSet.parse(value); } }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.Update, true)]
    public DateTime uDate { get; set; }
    [DbRowKey("sDate", RowAttribute.Time, DbAction.Update, true)]
    public DateTime sDate { get; set; }
    [DbRowKey("eDate", RowAttribute.Time, DbAction.Update, true)]
    public DateTime eDate { get; set; }
    [DbRowKey("UniqueID", RowAttribute.Guid, true)]
    public Guid UniqueID { get; set; }

    public DbObjList<OpenTime> OpenSet {
        get {
            openList.isNull(() =>
            {

                openList = new DbObjList<OpenTime>(EkiSql.ppyp.tableByPair<OpenTime>(QueryPair.New().addQuery("ParentId", Id)));
            });
            return openList;
        }
        set {
            openList = value;
        } 
    } 
    public CurrencyUnit Currency = CurrencyUnit.TWD;
    public PriceMethodSet PriceMethod = PriceMethodSet.Per30Min;

    private DbObjList<OpenTime> openList = null;

    public override bool CreatById(int id)
    {
        try
        {
            //var now = DateTime.Now;
            EkiSql.ppyp.loadDataById(id, this);

            //避免資料太多效率太慢
            //OpenSet = new DbObjList<OpenTime>(
            //    LoadListByQueryPair<OpenTime>(QueryPair.getInstance().addQuery("ParentId", id)));
            //OpenSet = new DbObjList<OpenTime>(from t in LoadListByQueryPair<OpenTime>(QueryPair.getInstance().addQuery("ParentId", id))
            //                                  where t.weekEnum==WeekEnum.NONE?true:t.getStartTime().Month>=now.Month
            //                                  select t);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public override bool CreatByUniqueId(string uniqueId)
    {
        try
        {
            EkiSql.ppyp.loadDataByQueryPair(QueryPair.New().addQuery("UniqueID", uniqueId), this);
            //OpenSet = new DbObjList<OpenTime>();
            //OpenSet.AddRange(LoadListByQueryPair<OpenTime>(QueryPair.getInstance().addQuery("ParentId", Id)));
            return true;
        }
        catch (Exception)
        {
            return false;
        }        
    }

    public override int Insert(bool isReturnId = false)
    {
        var id = EkiSql.ppyp.insert(this, true);
        if (id!=-1)
        {
            OpenSet.InsertToDb(set =>
            {
                set.ParentId = id;
            });
            return id;
        }
        return -1;
    }

    //有用到再改
    public override bool Update()
    {
        //獨立出來額外處理
        //OpenSet.InsertToDb(set =>
        //{
        //    set.ParentId = Id;
        //});
        return EkiSql.ppyp.update(this);
    }
    public override bool Delete()
    {
        if(OpenSet.DeleteInDb())        
            return EkiSql.ppyp.delete(this);
        return false;
    }

    public decimal price() => Price;

    public PriceMethodSet methodSet() => PriceMethod;

    public CurrencyUnit currencyUnit() => Currency;

    public void editBy(ReservaConfigRequest data,int version=1)
    {
        beEnable = data.beEnable;
        beRepeat = data.beRepeat;
        Text = data.text;
        Price = data.price;
        Unit = data.unit;
        Method = data.method;
    }
}