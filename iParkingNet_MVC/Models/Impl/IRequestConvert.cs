using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// RequestConvert 的摘要描述
/// </summary>
public interface  IRequestConvert<M>
{
     M convertToDbModel() ;
}