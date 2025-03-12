using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// IEdit 的摘要描述
/// </summary>
public interface IEdit<T>
{
    void editBy(T data,int version=1);
}