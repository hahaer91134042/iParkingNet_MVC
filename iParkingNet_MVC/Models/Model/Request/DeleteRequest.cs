using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// DeleteRequest 的摘要描述
/// </summary>
public class DeleteRequest : RequestAbstractModel
{
    public List<int> id { get; set; }
    public List<string> serNum { get; set; }

    public bool isIdEmpty()
    {
        if (id != null)
            return id?.Count <= 0;
        return true;
    }

    public bool isSerNumEmpty()
    {
        if (serNum != null)
            return serNum?.Count <= 0;
        return true;
    }
}