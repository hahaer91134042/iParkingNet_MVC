using System;
using System.Web;
using DevLibs;

public abstract class RequestAbstractModel : ApiAbstractModel
{        

    protected void cleanXssStr(string input) => input = TextUtil.cleanHtmlFragmentXss(input);

    public virtual Boolean cleanXss()
    {
        return false;
    }

    //判斷資料是否全部有效
    public virtual Boolean isValid()
    {
        return false;
    }

    public virtual Boolean isEmpty()
    {
        return true;
    }
}
