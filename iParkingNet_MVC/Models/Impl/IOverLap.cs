using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// IOverLap 的摘要描述
/// </summary>
public interface IOverLap<T>
{
    bool overlap(T other);
}