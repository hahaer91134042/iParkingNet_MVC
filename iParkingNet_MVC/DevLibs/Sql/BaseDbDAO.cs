using DevLibs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// BaseModel 的摘要描述
/// </summary>
public abstract class BaseDbDAO:DbOperationModel
{

    //[Newtonsoft.Json.JsonIgnore()]
    [DbRowKey("Id", RowAttribute.PrimaryKey, DbAction.Static, false)]
    public new int Id { get { return base.Id; } set { base.Id = value; } }

    public abstract bool CreatById(int id);
    public abstract int Insert(bool isReturnId=false);
    public virtual bool CreatByUniqueId(string uniqueId)
    {
        return false;
    }
    public virtual bool Update()
    {
        return false;
    }
    public virtual bool Delete()
    {
        return false;
    }


    //public static bool CreatByQuery<M>(QueryPair pair,M model)where M:BaseDbDAO
    //{
    //    return EkiSql.ppyp.loadDataByQueryPair(pair, model);
    //}

    //protected bool DeleteData<M>(M model)where M : BaseDbDAO
    //{
    //    return EkiSql.ppyp.delete(model);
    //}
    //protected bool LoadDataById<M>(int id, M model=null) where M : BaseDbDAO
    //{
    //    return EkiSql.ppyp.loadDataById(id, model);
    //}
    //protected M LoadDataById<M>(int id,M model=null)where M:BaseDbDAO
    //{
    //    using (var sqlHelper=new SqlHelper())
    //    {
    //        var cmd = SqlCmd.Get<M>.DataObjByIdCmd(id);
    //        return TableParaser.ConvertToObject<M>(sqlHelper.query(cmd), model);
    //    }
    //}
    //protected List<M> LoadListByQueryPair<M>(QueryPair pair)where M : BaseDbDAO
    //{
    //    return EkiSql.ppyp.tableByPair<M>(pair).toSafeList();
    //}
    //protected bool LoadDataByQueryPair<M>(QueryPair pair,M model=null)where M : BaseDbDAO
    //{
    //    return EkiSql.ppyp.loadDataByQueryPair(pair, model);
    //}
    //protected int InsertData<M>(M model,bool isReturnId)where M : BaseDbDAO
    //{
    //    return EkiSql.ppyp.insert(model, isReturnId);       
    //}

    //protected bool UpdateData<M>(M model) where M : BaseDbDAO
    //{
    //    return EkiSql.ppyp.update(model);
    //}

    public virtual Boolean isEmpty()
    {
        return true;
    }

    //protected IQueryable<T> GetTable<T>(SqlHelper sqlHelper) where T : DbOperationModel
    //{
    //    var cmd = SqlCmd.Get<T>.TableData();
    //    return TableParaser.ConvertQueryByRowName<T>(sqlHelper.query(cmd));
    //}
}