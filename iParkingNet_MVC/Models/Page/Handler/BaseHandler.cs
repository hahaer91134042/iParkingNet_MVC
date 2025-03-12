using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BaseHandler 的摘要描述
/// </summary>
public abstract class BaseHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        using (var sqlHelper=new SqlContext(EkiSql.ppyp))
        {
            GetRequest(context, sqlHelper);
        }            
    }

    protected abstract void GetRequest(HttpContext context, SqlContext sqlHelper);

    bool IHttpHandler.IsReusable => false;

}