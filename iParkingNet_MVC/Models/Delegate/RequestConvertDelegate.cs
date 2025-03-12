using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// RequestConvertDelegate 的摘要描述
/// </summary>
public delegate IRequestConvert<M> RequestConvertDelegate<M>(M request) where M : RequestAbstractModel;