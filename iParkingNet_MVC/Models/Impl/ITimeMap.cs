using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ITimeSet 的摘要描述
/// </summary>
public interface ITimeMap
{    
    int week();
    string date();
    string startTime();
    string endTime();
}