using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// CurrentUnit 電流種類
/// </summary>
public enum CurrentUnit
{
    Abort = -1,//只有使用了v2的才會出現這flag
    NONE =0,
    AC=1,
    DC=2
}