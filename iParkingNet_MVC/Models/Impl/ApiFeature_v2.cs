using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ApiFeature_v2 的摘要描述
/// </summary>
public class ApiFeature_v2
{
    public interface IConvertResponse<M>
    {
        M convertToResponse_v2();
    }
    public interface IRequestConvert<T>
    {
        T convertToModel_v2(Action<T> back=null);
    }
    public interface Request
    {
        bool isValid_v2();
    }
}