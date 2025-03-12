using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;

/// <summary>
/// Address 的摘要描述
/// </summary>
[DbTableSet("Address")]
public class Address : BaseDbDAO,
                                         IConvertResponse<AddressResponseModel>,
                                         IEdit<Address>
{
    [DbRowKey("Country",DbAction.Update)]
    public string Country { get; set; }
    [DbRowKey("State",DbAction.Update)]
    public string State { get; set; }
    [DbRowKey("City",DbAction.Update)]
    public string City { get; set; }
    [DbRowKey("Detail",DbAction.Update)]
    public string Detail { get; set; }
    [DbRowKey("ZipCode",DbAction.Update)]
    public string ZipCode { get; set; }

    public override bool CreatById(int id)
    {
        return EkiSql.ppyp.loadDataById(id, this);
    }

    public override int Insert(bool isReturnId = false)
    {
        return EkiSql.ppyp.insert(this, isReturnId);
    }
    public override bool Update()
    {
        return EkiSql.ppyp.update(this);
    }
    public override bool Delete()
    {
        return EkiSql.ppyp.delete(this);
    }

    public AddressResponseModel convertToResponse()
    {
        //var response = new AddressResponseModel();
        //response.load(this);
        return new AddressResponseModel().Also(a=>a.load(this));
    }

    public void editBy(Address data, int version = 1)
    {
        data.Id = Id;//把原本的ID固定設定上去 不然會錯誤
        data.copyTo(this);
    }
}