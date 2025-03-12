using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ResponseBuilder = DevLibs.ResponseBuilder;

/// <summary>
/// BaseWebPage 的摘要描述
/// </summary>
public abstract partial class BaseUserControl: System.Web.UI.UserControl
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
    protected IQueryable<T> GetTable<T>(SqlContext sqlHelper) where T : DbOperationModel
    {
        var cmd = SqlCmd.Get<T>.TableData();
        return TableParaser.ConvertQueryByRowName<T>(sqlHelper.query(cmd));
    }
}