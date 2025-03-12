using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// CheckProcess 的摘要描述
/// </summary>
public class CheckProcess : IRunable,IDisposable
{
    private List<ICheckValid> checkList = new List<ICheckValid>();

    public CheckProcess add(ICheckValid check)
    {
        checkList.Add(check);
        return this;
    }

    public void Dispose()
    {
        checkList.Clear();
    }

    public void run()
    {
        checkList.ForEach(check =>
        {
            check.validCheck();
        });        
    }
}