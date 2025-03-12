using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ResponseBuilder = DevLibs.ResponseBuilder;
using DevLibs;

/// <summary>
/// BaseMasterPage 的摘要描述
/// </summary>
public abstract partial class BaseMasterPage: System.Web.UI.MasterPage
{
    protected ResponseBuilder rBuilder = new ResponseBuilder();

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        using (var sqlHelper = new SqlContext(EkiSql.ppyp))
        {
            GetData(sqlHelper);
            CreatItem();
        }
    }
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
    }
    protected virtual void CreatItem()
    {

    }
    protected virtual void GetData(SqlContext sqlHelper)
    {

    }
}