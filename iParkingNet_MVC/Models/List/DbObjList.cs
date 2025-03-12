using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// DbObjList 的摘要描述
/// </summary>
public class DbObjList<M> : List<M> where M : BaseDbDAO
{
    public DbObjList(){}
    public DbObjList(IEnumerable<M> list)
    {
        AddRange(list);
    }
    public DbObjList(List<M> list)
    {
        AddRange(list);
    }

    public DbObjList<M> loadRequest<T>(List<T> list)where T:IRequestConvert<M>
    {
        list.ForEach(obj =>
        {
            Add(obj.convertToDbModel());            
        });
        return this;
    }
    public List<R> convertResponseList_v2<R>()
    {
        return (from data in this.AsEnumerable()
                where typeof(ApiFeature_v2.IConvertResponse<R>).IsAssignableFrom(data.GetType())
                select (R)data.GetType().GetMethod("convertToResponse_v2").Invoke(data, null)).toSafeList();
    }
    public List<R> convertResponseList<R>()
    {
        return (from data in this.AsEnumerable()
                where typeof(IConvertResponse<R>).IsAssignableFrom(data.GetType())
                select (R)data.GetType().GetMethod("convertToResponse").Invoke(data,null)).toSafeList();
    }

    public bool DeleteInDb()
    {
        try
        {
            var result = EkiSql.ppyp.deleteById<M>((from data in this
                                                    select data.Id).toSafeList());
            if (result)
                Clear();

            return result;
        }
        catch (Exception)
        {
            return false;
        }        
    }

    public bool UpdateByObj(Action<M> callBack = null)
    {
        try
        {
            ForEach(data =>
            {
                callBack?.Invoke(data);

                data.Update();
            });
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool InsertByObj(Action<M> callBack = null)
    {
        try
        {
            if (callBack != null)
            {
                ForEach(data =>
                {
                    callBack(data);
                });
            }

            ForEach(data =>
            {
                data.Insert();
            });
            return true;
        }
        catch (Exception)
        {
            return false;
        }        
    }
    public bool InsertToDb(Action<M> callBack=null)
    {
        try
        {
            if (callBack != null)
            {
                ForEach(data =>
                {
                    callBack(data);
                });
            }


            return EkiSql.ppyp.insert<M>(this);
        }
        catch (Exception)
        {
            return false;
        }        
    }
}