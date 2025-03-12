using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// CheckOutRequest 的摘要描述
/// </summary>
public class CheckOutRequest:RequestAbstractModel,IRequestConvert<EkiCheckOut>
{
    public string number { get; set; }
	public string date { get; set; }//yyyy-MM-dd hh:mm:ss
    public double lat { get; set; }
    public double lng { get; set; }

    public string action { get; set; }
    public string discount { get; set; }
    public Invoice invoice { get; set; }
    //public string lan = LanguageFamily.TC.ToString();
    //public string credit { get; set; }//信用卡末4碼

    public class Invoice:IConvertData<EkiInvoice>
    {
        public string name { get; set; }
        public string ubn { get; set; }//統一編號
        public string address { get; set; }
        public string mail { get; set; }
        public int type { get; set; } = 0;
        public string carrierNum { get; set; }
        public string loveCode { get; set; }

        public EkiInvoice convertData() => new EkiInvoice
        {
            Type=type,
            Name=name,
            BuyerUBN=ubn,
            Address=address,
            Email=mail,
            CarrierNum=carrierNum,
            LoveCode=loveCode
        };
    }

    public override bool cleanXss()
    {
        try
        {
            cleanXssStr(number);
            cleanXssStr(date);
            cleanXssStr(action);
            cleanXssStr(discount);
            //cleanXssStr(lan);
            //cleanXssStr(credit);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public EkiCheckOut convertToDbModel()
    {
        return new EkiCheckOut()
        {
            //OrderId=getDataRowId<EkiOrder>(QueryPair.getInstance().addQuery("SerialNumber",number)),
            Date = date.toDateTime(),
            Lat =lat,
            Lng=lng,
            Ip=WebUtil.GetUserIP()
            //CreditNum=credit
        };
    }

    public override bool isValid()
    {
        cleanXss();

        if (string.IsNullOrEmpty(number))
            return false;
        if (! date.isDateTime())
            return false;
        var order = EkiSql.ppyp.data<EkiOrder>(QueryPair.New().addQuery("SerialNumber", number));
        var isValid = false;

        order.notNull(ord =>
        {
            if (ord.Status <= OrderStatus.InUsing.toInt() && ord.beEnable)
            {
                isValid = ord.ReservaTime.getStartTime() < date.toDateTime(ApiConfig.DateTimeFormat);
            }
        });

        discount.notNullOrEmpty(dis =>
        {
            isValid = new DiscountCodeRule().isInRule(dis);
        });
        //action.notNullOrEmpty(act =>
        //{

        //});

        return isValid;
        //&& !string.IsNullOrEmpty(credit)
    }
}