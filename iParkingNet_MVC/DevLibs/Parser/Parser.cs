using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Parser 的摘要描述
/// </summary>
public interface Parser<T>
{
     void parse(T value);
}