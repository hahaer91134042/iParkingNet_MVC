using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ConvertResponse 的摘要描述
/// </summary>
public interface IConvertResponse<M>
{
    M convertToResponse();
}