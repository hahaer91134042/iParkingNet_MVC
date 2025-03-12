using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// IEcPayCheck 的摘要描述
/// </summary>
public interface IEcPayCheck
{
    string hashKey();
    string hashIV();
}