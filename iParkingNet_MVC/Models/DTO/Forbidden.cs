using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Forbidden 的摘要描述
/// </summary>
public class Forbidden
{
    public DateTime Start;
    public DateTime End;

    public Forbidden(DateTime start, DateTime end)
    {
        this.Start = start;
        this.End = end;
    }

    public bool IsBetween(DateTime time)
    {
        return Start < time && time < End;
    }
}