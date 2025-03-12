using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ITimeRange 的摘要描述
/// </summary>
public interface ITimeRange
{
    DateTime start();
    DateTime end();
}