using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BaseProcess 的摘要描述
/// </summary>
public abstract class BaseProcess : IRunable
{
    public abstract void run();
}