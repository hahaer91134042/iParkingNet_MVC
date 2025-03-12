using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// 表示停車位目前時間可不可用
/// </summary>
public enum AvailableStatus
{
    UnKnow=0,
    Available=1,//可用
    Unavailable = 2//不可用
}