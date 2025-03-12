using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

/// <summary>
/// EkiOrder 的摘要描述
/// </summary>
[DbTableSet("EkiOrder")]
public class EkiOrder:BaseDbDAO,IConvertResponse<EkiOrderResponse>,IPriceSet<decimal>,IRange<DateTime>
{
    [DbRowKey("LocationId",false)]
    public int LocationId { get; set; }
    [DbRowKey("AddressId",false)]
    public int AddressId { get; set; }
    [DbRowKey("MemberId", false)]
    public int MemberId { get; set; }
    [DbRowKey("ReservedId",false)]
    public int ReservedId { get; set; }
    [DbRowKey("SerialNumber",false)]
    public string SerialNumber { get; set; }
    [DbRowKey("LocPrice",DbAction.Update)]
    public decimal LocPrice { get; set; }
    [DbRowKey("Cost",DbAction.Update)]
    public decimal Cost { get; set; }
    [DbRowKey("Unit",DbAction.Update)]
    public int Unit { get; set; }
    [DbRowKey("Tax",DbAction.Update)]
    public decimal Tax { get; set; }
    [DbRowKey("HandlingFee",DbAction.Update)]//手續費
    public decimal HandlingFee { get; set; }
    [DbRowKey("Status",DbAction.Update)]
    public int Status { get { return convertEnumToInt(StatusEnum); } set { StatusEnum = convertIntToEnum<OrderStatus>(value); } }
    [DbRowKey("CarNum",false)]
    public string CarNum { get; set; }
    [DbRowKey("Remark",DbAction.Update)]//給後台人員備註用
    public string Remark { get; set; }
    [DbRowKey("cDate", RowAttribute.CreatTime, true)]
    public DateTime cDate { get; set; }
    [DbRowKey("uDate", RowAttribute.NowTime, DbAction.UpdateOnly, true)]
    public DateTime uDate { get; set; }
    [DbRowKey("sDate", RowAttribute.Time, DbAction.UpdateOnly, true)]
    public DateTime sDate { get; set; }
    [DbRowKey("eDate", RowAttribute.Time, DbAction.UpdateOnly, true)]
    public DateTime eDate { get; set; }
    [DbRowKey("UniqueID", RowAttribute.Guid, true)]
    public Guid UniqueID { get; set; }
    [DbRowKey("Ip", DbAction.Update, true)]
    public string Ip { get; set; }
    [DbRowKey("beEnable", DbAction.Update)]
    public bool beEnable { get; set; } = true;//操作規則:要刪除才=false cancel 不變

    public OrderStatus StatusEnum = OrderStatus.Reserved;
    public ReservedTime ReservaTime 
    { 
        get 
        {
            if (rTime == null)
                rTime = new ReservedTime().Also(r => r.CreatById(ReservedId));
            return rTime;
        }
        set { rTime = value; }
    }
    public Address Address
    {
        get
        {
            if (addr == null)
                addr = new Address().Also(a => a.CreatById(AddressId));
            return addr;
        }
        set { addr = value; }
    }

    public EkiCheckOut CheckOut
    {
        get
        {
            if (checkout == null)
                checkout = EkiSql.ppyp.data<EkiCheckOut>(
                    "where OrderId=@oid",
                    new { oid = Id });
                //checkout = new EkiCheckOut().Also(c => c.CreatedByOrderId(Id));
            return checkout;
        }
        set { checkout = value; }
    }
    public Location Location
    {
        get
        {
            if (loc == null)
                loc = new Location().Also(l => l.CreatById(LocationId));
            return loc;
        }
    }
    public EkiInvoice Invoice
    {
        get
        {
            if (_invo == null)
                _invo = EkiSql.ppyp.data<EkiInvoice>(
                    "where OrderId=@oid",
                    new { oid = Id });
            return _invo;
        }
    }
    public Member Member
    {
        get
        {
            if (_member == null)
                _member = EkiSql.ppyp.data<Member>(
                    "where Id=@mid",
                    new { mid = MemberId });
            return _member;
        }
    }
    

    public bool isCheckOut() => StatusEnum == OrderStatus.BeSettle || StatusEnum == OrderStatus.CheckOut;

    public DateTime getStartTime() => ReservaTime.getStartTime();
    public DateTime getEndTime()
    {
        if (isCheckOut()&&CheckOut!=null)
        {
            return CheckOut.Date.standarOrderReservaTime();
        }
        return ReservaTime.getEndTime();
    }


    private ReservedTime rTime;
    private Address addr;
    private EkiCheckOut checkout;
    private Location loc;
    private EkiInvoice _invo;
    private Member _member;

    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id,this);
    }

    public override int Insert(bool isReturnId = false)
    {
        ReservedId = ReservaTime.Insert(true);
        return EkiSql.ppyp.insert(this, isReturnId);
    }
    public EkiOrder cancelByManager()
    {
        StatusEnum = OrderStatus.CancelByManager;
        ReservaTime.IsCancel = true;
        return this;
    }
    public EkiOrder toCancel(double cost=0.0)
    {
        StatusEnum = OrderStatus.Cancel;        
        ReservaTime.IsCancel = true;
        Cost = cost.toDecimal();
        //ReservaTime.Update();//修改IsCancel
        return this;
    }

    public override bool Update()
    {
        //預約時間假如 checkout 要修改
        ReservaTime.Update();
        return EkiSql.ppyp.update(this);
    }

    public override bool Delete()
    {
        try
        {
            beEnable = false;
            ReservaTime.IsCancel = true;
            return Update();
        }
        catch(Exception e)
        {
            e.saveLog("Order error", "delete", this);
        }
        return false;
    }

    public override bool CreatByUniqueId(string uniqueId)
    {
        return EkiSql.ppyp.loadDataByQueryPair(QueryPair.New().addQuery("UniqueID", uniqueId), this);
    }

    public EkiOrderResponse convertToResponse()
    {
        string checkUrl="";
        EkiCheckOut checkOut = null;
        InvoiceResponse invoice = null;
        switch (StatusEnum)
        {
            case OrderStatus.Cancel:
                var cancel = (from c in EkiSql.ppyp.table<OrderCancel>()
                              where c.OrderId == Id
                              select c);
                checkUrl = cancel.Count() < 1 ? "" : cancel.FirstOrDefault().Cost > 0 && !cancel.FirstOrDefault().Paid? this.getPayUrl() : "";
                //ReservaTime.EndTime = cancel.Count() < 1 ? ReservaTime.EndTime : cancel.FirstOrDefault().Time;
                break;
            case OrderStatus.BeSettle:
                checkOut = (from c in EkiSql.ppyp.table<EkiCheckOut>()
                            where c.OrderId == Id
                            select c).FirstOrDefault();
                checkUrl = checkOut.Url;
                break;
            case OrderStatus.CheckOut:
                checkOut= (from c in EkiSql.ppyp.table<EkiCheckOut>()
                           where c.OrderId == Id
                           select c).FirstOrDefault();
                invoice = this.getInvoiceResponse();
                break;
            case OrderStatus.PayError:
                checkOut = (from c in EkiSql.ppyp.table<EkiCheckOut>()
                            where c.OrderId == Id
                            select c).FirstOrDefault();
                invoice = this.getInvoiceResponse();
                checkUrl = checkOut.Url;
                break;
            default:
                //checkUrl = (from c in Sql.table<EkiCheckOut>()
                //            where StatusEnum == OrderStatus.BeSettle
                //            where c.OrderId == Id
                //            select c.Url).FirstOrDefault();
                break;
        }

        return new EkiOrderResponse()
        {
            SerialNumber = SerialNumber,
            LocPrice=LocPrice.toDouble(),
            Cost = Cost,
            Unit = Unit,
            HandlingFee = HandlingFee,
            Status = Status,
            CarNum = CarNum,
            CreatTime = cDate.toString(),
            CheckOutUrl = checkUrl.isNullOrEmpty() ? "" : checkUrl,
            //ReservaTime = ReservaTime.convertToResponse(),
            ReservaTime=new ReservaTimeResponse
            {
                StartTime=getStartTime().toString(),
                //這邊因為checkout的時間也丟出了 所以丟原本的預約時間就好
                EndTime = ReservaTime.getEndTime().toString(),
                Remark=ReservaTime.Remark
            },
            Address = Address.convertToResponse(),
            Checkout=checkOut?.convertToResponse(),
            Invoice=invoice
            //Remark=Remark
        }.setLoc(new Location().Also(loc => loc.CreatById(LocationId)));
    }

    public decimal price() => LocPrice;
    public PriceMethodSet methodSet() => PriceMethodSet.Per30Min;
    public CurrencyUnit currencyUnit() => Unit.toEnum<CurrencyUnit>();

    public bool between(DateTime other)
    {
        var start = getStartTime();
        var end = getEndTime();

        return start <= other && other <= end;
    }
}