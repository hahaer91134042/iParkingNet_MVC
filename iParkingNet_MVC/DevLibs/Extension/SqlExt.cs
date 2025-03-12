using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Sql 的摘要描述
/// </summary>
namespace DevLibs
{
    public static class SqlExt
    {
        public static SqlConnection toSqlClient(this ISql sql) => new SqlConnection(sql.dbConfig().connetString());

        public static IEnumerable<M> table<M>(this ISql sql) where M : DbOperationModel
        {
            using (var sqlHelper = new SqlContext(sql))
            {
                var cmd = SqlCmd.Get<M>.tableDataByDapper();
                //Log.print($"Sql table cmd->{cmd}");
                //return TableParaser.ConvertQueryByRowName<M>(sqlHelper.query(cmd));
                return sqlHelper.query<M>(cmd, null);
            }
        }
        public static IEnumerable<M> table<M>(this ISql sql, string cmd, object param = null)
        {
            using (var helper = new SqlContext(sql))
            {
                return helper.query<M>(cmd, param);
            }
        }
        public static IEnumerable<M> tableByPair<M>(this ISql sql, QueryPair pair) where M : BaseDbDAO
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Get<M>.dataByPair(pair);
                    //return TableParaser.ConvertToListByRowName<M>(sqlHelper.query(cmd));
                    return helper.query<M>(cmd, null);
                }
            }
            catch (Exception) { }
            return new List<M>();
        }

        public static bool loadDataByQueryPair<M>(this ISql sql, QueryPair pair, M model = null) where M : BaseDbDAO
        {
            try
            {
                using (var sqlHelper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Get<M>.dataByPair(pair);
                    //Log.print($"cmd->{cmd}");
                    TableParaser.ConvertToObject<M>(sqlHelper.query(cmd), model);
                }
                return true;
            }
            catch { }
            return false;
        }
        public static bool loadDataById<M>(this ISql sql, int id, M model) where M : BaseDbDAO
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Get<M>.dataById(id);
                    //Log.d($"LoadById cmd->{cmd}");
                    //var table = sqlHelper.query(cmd);
                    //Log.d($"table count->{table.Rows.Count}");
                    TableParaser.ConvertToObject<M>(helper.query(cmd), model);
                    return true;
                }
            }
            catch (Exception e)
            {
                Log.d($"Sql load error ->{e.Message}");
                return false;
            }
        }

        /// <summary>
        /// where="where name=@name"
        /// param=new {name="test"}
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="sql"></param>
        /// <param name="where">sql where query string fragment</param>
        /// <param name="param">dapper query object</param>
        /// <returns></returns>
        public static M data<M>(this ISql sql, string where, object param = null) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var set = typeof(M).getAttribute<DbTableSet>();
                    var cmd = $"select * from [{set.TableName}] {where}";
                    //Log.d($"data cmd->{cmd} param->{param.toJsonString()}");
                    return helper.queryFirst<M>(cmd, param);
                }
            }
            catch (Exception e)
            {
                //Log.d($"get ex->{e.Message}");
            }
            return null;
        }
        public static M data<M>(this ISql sql, QueryPair pair) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Get<M>.dataByPair(pair);
                    //Log.d($"Sql data cmd->{cmd}");
                    return helper.queryFirst<M>(cmd);
                }
            }
            catch (Exception e)
            {
                //Log.d($"get ex->{e.Message}");
            }
            return null;
        }
        public static IEnumerable<M> dataById<M>(this ISql sql, List<int> list) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Get<M>.dataById(list);
                    //Log.d($"Sql dataById List cmd->{cmd}");
                    return helper.query<M>(cmd, null);
                }
            }
            catch (Exception e)
            {
                //Log.d($"get ex->{e.Message}");
            }
            return new List<M>();
        }
        public static M dataById<M>(this ISql sql, int id) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Get<M>.dataById(id);
                    //Log.d($"Sql dataById cmd->{cmd}");
                    return helper.queryFirst<M>(cmd);
                }
            }
            catch (Exception e)
            {
                //Log.d($"get ex->{e.Message}");
            }
            return null;
        }
        public static int count<M>(this ISql sql, string where, object param = null) where M : DbOperationModel
        {
            var set = typeof(M).getAttribute<DbTableSet>();
            var cmd = $"select count(*) from [{set.TableName}] {where}";
            return sql.count(cmd, param);
        }
        /// <summary>
        /// 需使用 select COUNT(*) from table
        /// </summary>
        /// <returns></returns>
        public static int count(this ISql sql, string cmd, object param = null)
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    return helper.queryFirst<int>(cmd, param);
                }
            }
            catch (Exception e)
            {
                Log.e("Sql count error", e);
            }
            return -1;
        }

        public static int count<M>(this ISql sql, QueryPair pair = null) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    if (pair == null)
                    {
                        return helper.queryFirst<int>(SqlCmd.Get<M>.rowCount());
                    }
                    else
                    {
                        var cmd = SqlCmd.Get<M>.RowCountCmd(pair);
                        //Log.d($"count cmd->{cmd}");
                        return TableParaser.CountNum(helper.query(cmd));
                    }
                }
            }
            catch (Exception e)
            {
                Log.e("Sql count error", e);
            }
            return -1;
        }
        public static bool hasData<M>(this ISql sql, string where, object param = null) where M : DbOperationModel
            => sql.count<M>(where, param) > 0;
        public static bool hasData(this ISql sql, string cmd, object param = null)
            => sql.count(cmd, param) > 0;
        public static bool hasData<M>(this ISql sql, QueryPair pair = null) where M : DbOperationModel
            => sql.count<M>(pair) > 0;
        public static bool insert<M>(this ISql sql, List<M> list) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    if (list.Count() > 0)
                    {
                        var cmd = SqlCmd.InsertTo<M>.ObjTable(list);
                        return helper.execute(cmd, null) > 0;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                Log.e("Insert error", e);
            }
            return false;
        }
        public static int insert<M>(this ISql sql, M data, bool isReturnId = false) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.InsertTo<M>.ObjTable(data, isReturnId);
                    //Log.d($"insert cmd->{cmd}");
                    if (isReturnId)
                    {
                        return helper.queryFirst<int>(cmd);
                    }
                    else
                    {
                        return helper.execute(cmd);
                    }
                }
            }
            catch (Exception e)
            {
                Log.e("Insert Error", e);
            }
            return -1;
        }

        public static bool insert<M>(this ISql sql, string cmd, object param) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    return helper.execute(cmd, param) > 0;
                }
            }
            catch (Exception e)
            {
                Log.e("Insert Error", e);
            }
            return false;
        }

        /// <summary>
        /// use cmd to update row or table
        /// exp:"set xxx=ooo where Id=1"
        /// </summary>
        /// <typeparam name="M"></typeparam>
        /// <param name="sql"></param>
        /// <param name="sqlStr">SQL set and where cmd string</param>
        /// <param name="param">dapper param object</param>
        /// <param name="model">where by Id</param>
        /// <returns></returns>
        public static bool update<M>(this ISql sql,string sqlStr,object param=null,M model=null) where M : DbOperationModel
        {
            var set = typeof(M).getAttribute<DbTableSet>();
            var where = model == null ? "" : $"where Id={model.Id}";
            var cmd = $"update [{set.TableName}] {sqlStr} {where}";
            return sql.update(cmd, param);
        }
        public static bool update(this ISql sql,string cmd,object param=null)
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    return helper.execute(cmd,param) > 0;
                }
            }
            catch (Exception e)
            {
                Log.e("Update error", e);
            }
            return false;
        }
        public static bool update<M>(this ISql sql, M data) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Update<M>.dataById(data);
                    //Log.d($"update cmd->{cmd}");
                    return helper.execute(cmd) > 0;
                }
            }
            catch (Exception e)
            {
                Log.e("UpdateById error", e);
            }
            return false;
        }

        public static bool updateList<M>(this ISql sql, List<M> list) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Update<M>.dataList(list);
                    return helper.execute(cmd) > 0;
                }
            }
            catch (Exception e)
            {
                Log.e("UpdateList error", e);
            }
            return false;
        }
        public static bool deleteById<M>(this ISql sql, List<int> list) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Delete<M>.TableRowById(list);
                    return helper.execute(cmd) > 0;
                }
            }
            catch
            {
                return false;
            }
        }
        public static bool deleteById<M>(this ISql sql, int id) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Delete<M>.TableRowById(id);
                    return helper.execute(cmd) > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool delete<M>(this ISql sql, M data) where M : DbOperationModel => sql.deleteById<M>(data.Id);
        public static bool delete<M>(this ISql sql, QueryPair pair) where M : DbOperationModel
        {
            try
            {
                using (var helper = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Delete<M>.TableRow(pair);
                    return helper.execute(cmd) > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool counterAddById<M>(this ISql sql, M data, int offset = 1) where M : DbOperationModel
            => sql.counterAddById<M>(data.Id, offset);
        public static bool counterAddById<M>(this ISql sql, int id, int offset = 1) where M : DbOperationModel
        {
            try
            {
                using (var con = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Counter<M>.addById(id, offset);
                    //Log.d($"counter add cmd->{cmd}");
                    return con.execute(cmd) > 0;
                }
            }
            catch (Exception e)
            {
                //Log.e("counterAddById", e);
                return false;
            }
        }
        public static bool counterAddByQuery<M>(this ISql sql, QueryPair q, int offset = 1) where M : DbOperationModel
        {
            try
            {
                using (var con = new SqlContext(sql))
                {
                    var cmd = SqlCmd.Counter<M>.addByQuery(q, offset);
                    //Log.d($"counter add cmd->{cmd}");
                    return con.execute(cmd) > 0;
                }
            }
            catch (Exception e)
            {
                //Log.e("counterAddByQuery", e);
                return false;
            }
        }
    }
}
