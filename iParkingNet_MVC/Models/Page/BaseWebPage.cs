using DevLibs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using ResponseBuilder = DevLibs.ResponseBuilder;

/// <summary>
/// BaseWebPage 的摘要描述
/// </summary>
public abstract partial class BaseWebPage:Page
{
    protected ResponseBuilder rBuilder = new ResponseBuilder();
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);        
    }
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

    protected override void OnLoadComplete(EventArgs e)
    {
        base.OnLoadComplete(e);
    }
    protected virtual void CreatItem()
    {

    }
    protected virtual void GetData(SqlContext sqlHelper)
    {

    }

    protected IEnumerable<T> GetTable<T>()where T:DbOperationModel
    {
        //var cmd = SqlCmd.Get<T>.TableData();
        //return TableParaser.ConvertQueryByRowName<T>(sqlHelper.query(cmd));
        return EkiSql.ppyp.table<T>();
    }

    protected CTRL LoadUserControl<CTRL>(string path)where CTRL : UserControl
    {
        return LoadControl(path) as CTRL;
    }

    protected CTRL FindControlFrom<CTRL>(Control from,string id)where CTRL : Control
    {
        return from.FindControl(id) as CTRL;
    }

    protected CTRL FindMasterControll<CTRL>(string id)where CTRL:Control
    {
        return (CTRL)Master.FindControl(id);
    }
}