using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace DevLibs
{
    public class MultiRow
    {
        public string key;
        public List<string> list;
        public MultiRow(string key, List<string> list)
        {
            this.key = key;
            this.list = list;
        }
    }

    public class RowRange
    {
        public int start;
        public int end;
        public int offset, fetch;

        public RowRange(int start, int end)
        {
            if (start < 1 || end < start)
                throw new ArgumentException("RowRange Start must large than 1 or End must large than Start!!");

            this.start = start;
            this.end = end;
            offset = start - 1;//因為row從一開始計
            fetch = end - start + 1;
        }

        public static RowRange Add(int start, int end)
        {
            return new RowRange(start, end);
        }
    }
    public class RowOrderBy
    {
        public Dictionary<string, OrderBy> pairs = new Dictionary<string, OrderBy>();

        public RowOrderBy(string key, OrderBy order)
        {
            pairs.Add(key, order);
        }

        public RowOrderBy Append(string key, OrderBy order = OrderBy.ASC)
        {
            pairs.Add(key, order);
            return this;
        }

        public static RowOrderBy Add(string key, OrderBy order = OrderBy.ASC)
        {
            return new RowOrderBy(key, order);
        }
    }

    public class QueryDateBetween
    {
        public string start;
        public string end;
        public DateUnit unit;

        public QueryDateBetween(string start, string end, DateUnit unit)
        {
            this.start = start;
            this.end = end;
            this.unit = unit;
        }
        //---當下時間跟兩個時間的欄位比較 是否在其之間
        public static QueryDateBetween Add(string start, string end, DateUnit unit = DateUnit.DAY)
        {
            return new QueryDateBetween(start, end, unit);
        }
    }

    public class QueryRange
    {
        public int min;
        public int max;

        public QueryRange(int min, int max)
        {
            this.min = min;
            this.max = max;
            if (min > max)
                throw new ArgumentException("Query Range min>max");
        }
        public static QueryRange Add(Enum min, Enum max)
        {
            return Add(Convert.ToInt32(min), Convert.ToInt32(max));
        }
        public static QueryRange Add(int min, int max)
        {
            return new QueryRange(min, max);
        }
    }

    public class QueryPair
    {
        public bool hasRowRange = false, hasRowOrderBy = false;
        public RowRange rowRange;
        public RowOrderBy rowOrderBy;
        private Dictionary<string, object> pair = new Dictionary<string, object>();
        private Dictionary<string, QueryRange> rangePair = new Dictionary<string, QueryRange>();
        private List<QueryDateBetween> dateList = new List<QueryDateBetween>();
        private MultiRow multiRow;

        #region Static method
        public static QueryPair New()
        {
            return new QueryPair();
        }
        public static QueryPair init(MultiRow multi)
        {
            return New().addQuery(multi);
        }

        public static QueryPair init(string key, object value)
        {
            return New().addQuery(key,value);
        }

        public static QueryPair init(string key, QueryRange range)
        {
            return New().addQuery(key, range);
        }

        public static QueryPair init(QueryDateBetween queryDate)
        {
            return New().addQuery(queryDate);
        }

        public static QueryPair init(RowRange rowRange)
        {
            return New().addQuery(rowRange);
        }

        public static QueryPair init(RowOrderBy rowOrderBy)
        {
            return New().addQuery(rowOrderBy);
        }
        #endregion


        public QueryPair addQuery(MultiRow multi)
        {
            multiRow = multi;
            return this;
        }

        public QueryPair addQuery(string key, object value)
        {
            if (pair.ContainsKey(key) || rangePair.ContainsKey(key))
                throw new SqlPairException();
            pair.Add(key, value);
            return this;
        }

        public QueryPair addQuery(string key, QueryRange range)
        {
            if (pair.ContainsKey(key) || rangePair.ContainsKey(key))
                throw new SqlPairException();
            rangePair.Add(key, range);
            return this;
        }

        public QueryPair addQuery(QueryDateBetween queryDate)
        {
            dateList.Add(queryDate);
            return this;
        }

        public QueryPair addQuery(RowRange rowRange)
        {
            hasRowRange = true;
            this.rowRange = rowRange;
            return this;
        }

        public QueryPair addQuery(RowOrderBy rowOrderBy)
        {
            hasRowOrderBy = true;
            this.rowOrderBy = rowOrderBy;
            return this;
        }

        public string getQueryStr()
        {
            #region ---WHERE String---
            var builder = new StringBuilder(" WHERE ");
            for (int i = 0; i < pair.Count; i++)
            {
                if (i < 1)
                    builder.Append("[" + pair.ElementAt(i).Key + "]='" + pair.ElementAt(i).Value + "'");
                else
                    builder.Append(" AND [" + pair.ElementAt(i).Key + "]='" + pair.ElementAt(i).Value + "'");
            }
            for (int i = 0; i < rangePair.Count; i++)
            {
                if (pair.Count > 0)
                    builder.Append(" AND ");
                builder.Append(" [" + rangePair.ElementAt(i).Key + "]>='" + rangePair.ElementAt(i).Value.min + "'");
                builder.Append(" AND [" + rangePair.ElementAt(i).Key + "]<='" + rangePair.ElementAt(i).Value.max + "'");
            }
            if (multiRow != null)
            {
                if (pair.Count > 0 || rangePair.Count > 0)
                    builder.Append(" AND ");
                builder.Append(" [" + multiRow.key + "] IN (");
                multiRow.list.ForEach(value =>
                {
                    builder.Append("'" + value + "',");
                });
                builder.Remove(builder.Length - 1, 1);
                builder.Append(") ");
            }

            var dateStr = new StringBuilder();
            dateList.ForEach(queryDate =>
            {
                if (!string.IsNullOrEmpty(queryDate.start))
                {
                    if (pair.Count > 0 || rangePair.Count > 0 || dateStr.Length > 0 || multiRow != null)
                        dateStr.Append(" AND ");

                    dateStr.Append(" DATEDIFF(" + queryDate.unit.ToString() + ",ISNULL([" + queryDate.start + "],GETDATE()),GETDATE())>=0");
                }

                if (!string.IsNullOrEmpty(queryDate.end))
                {
                    if (pair.Count > 0 || rangePair.Count > 0 || dateStr.Length > 0 || multiRow != null)
                        dateStr.Append(" AND ");

                    dateStr.Append(" DATEDIFF(" + queryDate.unit.ToString() + ",GETDATE(),ISNULL([" + queryDate.end + "],GETDATE()))>=0");
                }
            });
            builder.Append(dateStr.ToString());
            #endregion

            #region ---Order By String---
            if (hasRowOrderBy)
            {
                var orderStr = new StringBuilder(" ORDER BY ");
                foreach (var pair in rowOrderBy.pairs)
                {
                    orderStr.Append("[" + pair.Key + "] " + pair.Value.ToString() + ",");
                }
                orderStr.Remove(orderStr.Length - 1, 1);
                builder.Append(orderStr.ToString());
            }
            #endregion

            #region ---RowRange String---
            if (hasRowRange)
                builder.Append(" OFFSET " + rowRange.offset + " ROWS FETCH FIRST " + rowRange.fetch + " ROWS ONLY");
            #endregion


            return builder.ToString();
        }

        public Dictionary<string, object> getPairs()
        {
            return pair;
        }
    }

    public class SqlCmd
    {
        public static class Delete<MODEL> where MODEL : DbOperationModel
        {

            public static string TableRowById(List<int> list)
            {
                var properties = typeof(MODEL).GetProperties();
                var rowPKkey = "Id";
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;

                    //var key = dbRow.Key;
                    //var rowAttr = dbRow.Attribute;
                    if (!dbRow.Attribute.Equals(RowAttribute.PrimaryKey)) continue;
                    rowPKkey = dbRow.Key;
                }
                return TableRow(QueryPair.init(new MultiRow(rowPKkey, list.ConvertAll(new Converter<int, string>(Convert.ToString)))));
            }

            public static string TableRowById(int id)
            {
                var properties = typeof(MODEL).GetProperties();
                var rowPKkey = "Id";
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;

                    //var key = dbRow.Key;
                    //var rowAttr = dbRow.Attribute;
                    if (!dbRow.Attribute.Equals(RowAttribute.PrimaryKey)) continue;
                    rowPKkey = dbRow.Key;
                }
                return TableRow(QueryPair.New().addQuery(rowPKkey, id));
            }

            public static string TableRow(QueryPair pair)
            {
                try
                {
                    StringBuilder cmd = new StringBuilder("DELETE ");
                    var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;

                    var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                    cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                    cmd.Append(pair.getQueryStr());

                    return cmd.ToString();
                }
                catch (Exception)
                {
                    throw new DbModelException();
                }
            }
        }

        public static class Update<MODEL> where MODEL : DbOperationModel
        {
            //        UPDATE t1
            //        SET t1.StartValue = t2.StartValue,
            //            t1.EndValue= t2.EndValue
            //        FROM[iso2018].[dbo].[tcFactorValue] t1
            //        JOIN (
            //            VALUES        
            //                ('27','10', '1101'),
            //                ('28','1102', '2200')
            //        ) t2(Id, StartValue, EndValue) ON t2.Id = t1.Id
            public static string TableByList(List<MODEL> list)
            {

                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                StringBuilder cmd = new StringBuilder("UPDATE t1 SET ");
                var properties = typeof(MODEL).GetProperties();
                var rowPKkey = "Id";
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    //var dbRow = property.GetCustomAttribute<DbRowKey>();

                    if (dbRow.Action != DbAction.Update &&
                        dbRow.Action != DbAction.UpdateOnly) continue;

                    cmd.Append("t1." + dbRow.Key + "=t2." + dbRow.Key + ",");

                }
                cmd.Remove(cmd.Length - 1, 1);

                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] t1 JOIN ( VALUES ");

                list.ForEach(data =>
                {
                    var valuesFrag = new StringBuilder("(");
                    foreach (var property in properties)
                    {
                        if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                        var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;

                        //是PK值 跳過這步驟
                        if (dbRow.Attribute != RowAttribute.PrimaryKey)
                            if (dbRow.Action != DbAction.Update &&
                                dbRow.Action != DbAction.UpdateOnly) continue;

                        var value = property.GetValue(data, null);
                        if (!dbRow.Nullable && isEmptyStrOrZero(value))
                            throw new RowNoValueException("Id->" + data.Id + " property->" + dbRow.Key + " can`t be null value!!");


                        switch (dbRow.Attribute)
                        {
                            case RowAttribute.NowTime:
                                valuesFrag.Append("GETDATE(),");
                                break;
                            default:
                                //valuesFrag.Append("'" +value + "',");
                                valuesFrag.Append(value == null ? "null," : "'" + value + "',");
                                break;
                        }
                    }
                    valuesFrag.Remove(valuesFrag.Length - 1, 1);
                    valuesFrag.Append(")");
                    cmd.Append(valuesFrag.ToString() + ",");
                });
                cmd.Remove(cmd.Length - 1, 1);
                cmd.Append(") t2 ");

                var t2ColumFrag = new StringBuilder("(");
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    //是PK值 跳過這步驟
                    if (dbRow.Attribute != RowAttribute.PrimaryKey)
                        if (dbRow.Action != DbAction.Update &&
                            dbRow.Action != DbAction.UpdateOnly) continue;

                    t2ColumFrag.Append(dbRow.Key + ",");
                    switch (dbRow.Attribute)
                    {
                        case RowAttribute.PrimaryKey:
                            rowPKkey = dbRow.Key;
                            break;
                    }
                }
                t2ColumFrag.Remove(t2ColumFrag.Length - 1, 1);
                t2ColumFrag.Append(") ");
                cmd.Append(t2ColumFrag.ToString());
                cmd.Append("ON t2." + rowPKkey + " = t1." + rowPKkey);

                return cmd.ToString();
            }

            public static string dataList(List<MODEL> list)
            {

                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                StringBuilder cmd = new StringBuilder("UPDATE t1 SET ");
                var properties = typeof(MODEL).GetProperties();
                var rowPKkey = "Id";
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    //var dbRow = property.GetCustomAttribute<DbRowKey>();

                    if (dbRow.Action != DbAction.Update &&
                        dbRow.Action != DbAction.UpdateOnly) continue;

                    cmd.Append("t1." + dbRow.Key + "=t2." + dbRow.Key + ",");

                }
                cmd.Remove(cmd.Length - 1, 1);

                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] t1 JOIN ( VALUES ");

                list.ForEach(data =>
                {
                    var valuesFrag = new StringBuilder("(");
                    foreach (var property in properties)
                    {
                        if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                        var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;

                        //是PK值 跳過這步驟
                        if (dbRow.Attribute != RowAttribute.PrimaryKey)
                            if (dbRow.Action != DbAction.Update &&
                                dbRow.Action != DbAction.UpdateOnly) continue;

                        var value = property.GetValue(data, null);
                        if (!dbRow.Nullable && isEmptyStrOrZero(value))
                            throw new RowNoValueException("Id->" + data.Id + " property->" + dbRow.Key + " can`t be null value!!");


                        switch (dbRow.Attribute)
                        {
                            case RowAttribute.NowTime:
                                valuesFrag.Append("GETDATE(),");
                                break;
                            default:
                                if (value is DateTime time)
                                    value = time.toSqlTime().ToString(TimeUtil.DateTimeFormat);
                                valuesFrag.Append("'" + value + "',");
                                break;
                        }
                    }
                    valuesFrag.Remove(valuesFrag.Length - 1, 1);
                    valuesFrag.Append(")");
                    cmd.Append(valuesFrag.ToString() + ",");
                });
                cmd.Remove(cmd.Length - 1, 1);
                cmd.Append(") t2 ");

                var t2ColumFrag = new StringBuilder("(");
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    //是PK值 跳過這步驟
                    if (dbRow.Attribute != RowAttribute.PrimaryKey)
                        if (dbRow.Action != DbAction.Update &&
                            dbRow.Action != DbAction.UpdateOnly) continue;

                    t2ColumFrag.Append(dbRow.Key + ",");
                    switch (dbRow.Attribute)
                    {
                        case RowAttribute.PrimaryKey:
                            rowPKkey = dbRow.Key;
                            break;
                    }
                }
                t2ColumFrag.Remove(t2ColumFrag.Length - 1, 1);
                t2ColumFrag.Append(") ");
                cmd.Append(t2ColumFrag.ToString());
                cmd.Append("ON t2." + rowPKkey + " = t1." + rowPKkey);

                return cmd.ToString();
            }

            public static string dataById(MODEL data)
            {
                //var tableSet = typeof(T).GetCustomAttribute<DbTableSet>();
                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;

                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                StringBuilder cmd = new StringBuilder("UPDATE " + dbStr + "[" + tableSet.TableName + "] SET ");
                var properties = typeof(MODEL).GetProperties();
                var rowPKkey = "Id";
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    //var dbRow = property.GetCustomAttribute<DbRowKey>();
                    //是PK值 跳過這步驟
                    if (dbRow.Attribute != RowAttribute.PrimaryKey)
                        if (dbRow.Action != DbAction.Update &&
                            dbRow.Action != DbAction.UpdateOnly) continue;

                    var value = property.GetValue(data, null);
                    if (!dbRow.Nullable && isEmptyStrOrZero(value))
                        throw new RowNoValueException("property->" + dbRow.Key + " can`t be null value!!");

                    //if (!isEmptyStrOrZero(value) || !dbRow.Nullable)
                    //{
                    switch (dbRow.Attribute)
                    {
                        case RowAttribute.PrimaryKey:
                            rowPKkey = dbRow.Key;
                            break;
                        case RowAttribute.NowTime:
                            cmd.Append("[" + dbRow.Key + "]= GETDATE(),");
                            break;
                        default:
                            if (value is DateTime time)
                                value = time.toSqlTime().ToString(TimeUtil.DateTimeFormat);
                            cmd.Append("[" + dbRow.Key + "]='" + value + "',");
                            break;
                    }
                    //}
                }
                cmd.Remove(cmd.Length - 1, 1);
                cmd.Append(" WHERE [" + rowPKkey + "]='" + data.Id + "'");
                //Log.d($"update cmd->{cmd}");
                return cmd.ToString();
            }

            public static string ObjTableById(MODEL data)
            {
                //var tableSet = typeof(T).GetCustomAttribute<DbTableSet>();
                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;

                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                StringBuilder cmd = new StringBuilder("UPDATE " + dbStr + "[" + tableSet.TableName + "] SET ");
                var properties = typeof(MODEL).GetProperties();
                var rowPKkey = "Id";
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    //var dbRow = property.GetCustomAttribute<DbRowKey>();
                    //是PK值 跳過這步驟
                    if (dbRow.Attribute != RowAttribute.PrimaryKey)
                        if (dbRow.Action != DbAction.Update &&
                            dbRow.Action != DbAction.UpdateOnly) continue;

                    var value = property.GetValue(data, null);
                    if (!dbRow.Nullable && isEmptyStrOrZero(value))
                        throw new RowNoValueException("property->" + dbRow.Key + " can`t be null value!!");

                    //if (!isEmptyStrOrZero(value) || !dbRow.Nullable)
                    //{
                    switch (dbRow.Attribute)
                    {
                        case RowAttribute.PrimaryKey:
                            rowPKkey = dbRow.Key;
                            break;
                        case RowAttribute.NowTime:
                            cmd.Append("[" + dbRow.Key + "]= GETDATE(),");
                            break;
                        default:
                            //cmd.Append("[" + dbRow.Key + "]='" + value + "',");
                            //這似乎是C#在字串內部的三元判斷的問題
                            cmd.Append($"[{dbRow.Key}]=");
                            cmd.Append(value == null ? "null," : "'" + value + "',");
                            break;
                    }
                    //}
                }
                cmd.Remove(cmd.Length - 1, 1);
                cmd.Append(" WHERE [" + rowPKkey + "]='" + data.Id + "'");

                return cmd.ToString();
            }
        }

        public static class InsertTo<MODEL> where MODEL : DbOperationModel
        {
            public static string ObjTable(List<MODEL> dataList)
            {
                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                string cmd = "INSERT INTO " + dbStr + "[" + tableSet.TableName + "] ";
                var properties = typeof(MODEL).GetProperties();
                StringBuilder rowKeyStr = new StringBuilder(" (");
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;

                    DbRowKey dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    var action = dbRow.Action;
                    if (action.Equals(DbAction.Static) ||
                        action.Equals(DbAction.UpdateOnly) ||
                        dbRow.Attribute.Equals(RowAttribute.PrimaryKey)) continue;


                    rowKeyStr.Append("[" + dbRow.Key + "],");
                }
                rowKeyStr.Remove(rowKeyStr.Length - 1, 1);
                rowKeyStr.Append(") ");

                StringBuilder valueStr = new StringBuilder(" VALUES ");
                dataList.ForEach(data =>
                {
                    valueStr.Append("(");
                    foreach (var property in data.GetType().GetProperties())
                    {
                        if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                        DbRowKey dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                        var action = dbRow.Action;
                        if (action.Equals(DbAction.Static) ||
                            action.Equals(DbAction.UpdateOnly) ||
                            dbRow.Attribute.Equals(RowAttribute.PrimaryKey)) continue;

                        switch (dbRow.Attribute)
                        {
                            case RowAttribute.CreatTime:
                            case RowAttribute.NowTime:
                                valueStr.Append("GETDATE(),");
                                break;
                            case RowAttribute.Guid:
                                valueStr.Append("NEWID(),");
                                break;
                            default:
                                var value = property.GetValue(data, null);
                                if (!dbRow.Nullable && isEmptyStrOrZero(value))
                                    throw new RowNoValueException("Property->" + dbRow.Key + " not set value!!");
                                if (value is DateTime time)
                                    value = time.toSqlTime().ToString(TimeUtil.DateTimeFormat);
                                valueStr.Append(value == null ? "null," : "'" + value + "',");
                                break;
                        }
                    }
                    valueStr.Remove(valueStr.Length - 1, 1);
                    valueStr.Append("),");
                });

                valueStr.Remove(valueStr.Length - 1, 1);
                cmd += rowKeyStr.ToString() + valueStr.ToString();

                return cmd;
            }

            public static string ObjTable(MODEL data, Boolean isReturnId = false)
            {
                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                string cmd = "INSERT INTO " + dbStr + "[" + tableSet.TableName + "] ";
                var properties = data.GetType().GetProperties();
                StringBuilder rowKeyStr = new StringBuilder("(");
                StringBuilder valueStr = new StringBuilder(" VALUES (");

                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;

                    DbRowKey dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    var action = dbRow.Action;
                    if (action.Equals(DbAction.Static) ||
                        action.Equals(DbAction.UpdateOnly) ||
                        dbRow.Attribute.Equals(RowAttribute.PrimaryKey)) continue;

                    //var key = dbRow.Key;
                    //var rowAttr = dbRow.Attribute;
                    //var isNullable = dbRow.Nullable;                

                    rowKeyStr.Append("[" + dbRow.Key + "],");

                    switch (dbRow.Attribute)
                    {
                        case RowAttribute.CreatTime:
                        case RowAttribute.NowTime:
                            valueStr.Append("GETDATE(),");
                            break;
                        case RowAttribute.Guid:
                            valueStr.Append("NEWID(),");
                            break;
                        default:
                            var value = property.GetValue(data, null);
                            if (!dbRow.Nullable && isEmptyStrOrZero(value))
                                throw new RowNoValueException("Property->" + dbRow.Key + " not set value!!");
                            if (value is DateTime time)
                                value = time.toSqlTime().ToString(TimeUtil.DateTimeFormat);
                            valueStr.Append(value == null ? "null," : "'" + value + "',");
                            break;
                    }
                }
                rowKeyStr.Remove(rowKeyStr.Length - 1, 1);
                rowKeyStr.Append(") ");
                valueStr.Remove(valueStr.Length - 1, 1);
                valueStr.Append(") ");

                cmd += rowKeyStr.ToString() + valueStr.ToString();
                if (isReturnId)
                    cmd += " SELECT @@IDENTITY as Id";
                return cmd;
            }


        }

        public static class Get<MODEL> where MODEL : DbOperationModel
        {
            public static string tableDataByDapper()
            {
                try
                {
                    StringBuilder cmd = new StringBuilder("SELECT * ");
                    var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;

                    var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                    cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                    return cmd.ToString();
                }
                catch (Exception)
                {
                    throw new DbModelException();
                }
            }
            [Obsolete]
            public static string TableData()
            {
                try
                {
                    StringBuilder cmd = new StringBuilder("SELECT ");
                    var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                    var properties = typeof(MODEL).GetProperties();

                    foreach (var property in properties)
                    {
                        if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                        var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;

                        switch (dbRow.Attribute)
                        {
                            case RowAttribute.CreatTime:
                            case RowAttribute.NowTime:
                            case RowAttribute.Time:
                                cmd.Append("CONVERT(varchar(20),[" + dbRow.Key + "],120) AS " + dbRow.Key + ",");
                                break;
                            case RowAttribute.Guid:
                                cmd.Append("CONVERT(varchar(36), [" + dbRow.Key + "]) AS " + dbRow.Key + ",");
                                break;
                            default:
                                cmd.Append("[" + dbRow.Key + "],");
                                break;
                        }
                    }
                    cmd.Remove(cmd.Length - 1, 1);

                    var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                    cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                    return cmd.ToString();
                }
                catch (Exception)
                {
                    throw new DbModelException();
                }
            }

            public static string rowCount()
            {
                StringBuilder cmd = new StringBuilder("SELECT COUNT(*) ");
                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                return cmd.ToString();
            }

            public static string RowCountCmd(QueryPair paire = null)
            {
                StringBuilder cmd = new StringBuilder("SELECT COUNT(*) [count] ");
                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                if (paire != null)
                    cmd.Append(paire.getQueryStr());
                return cmd.ToString();
            }

            [Obsolete]
            public static string RowCountCmd(Dictionary<string, object> pairs)
            {
                StringBuilder cmd = new StringBuilder("SELECT COUNT(*) [count] ");
                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                for (int i = 0; i < pairs.Count; i++)
                {
                    if (i < 1)
                        cmd.Append(" WHERE [" + pairs.ElementAt(i).Key + "]='" + pairs.ElementAt(i).Value + "'");
                    else
                        cmd.Append(" AND [" + pairs.ElementAt(i).Key + "]='" + pairs.ElementAt(i).Value + "'");

                }
                return cmd.ToString();
            }

            //use dapper
            public static string dataByPair(QueryPair pair)
            {
                try
                {
                    StringBuilder cmd = new StringBuilder("SELECT ");
                    var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                    var properties = typeof(MODEL).GetProperties();

                    foreach (var property in properties)
                    {
                        if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                        var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                        //var key = dbRow.Key;
                        //var rowAttr = dbRow.Attribute;
                        cmd.Append("[" + dbRow.Key + "],");
                    }

                    cmd.Remove(cmd.Length - 1, 1);
                    var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                    cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                    cmd.Append(pair.getQueryStr());

                    return cmd.ToString();
                }
                catch (Exception)
                {
                    throw new DbModelException();
                }
            }

            [Obsolete]
            public static string DataObjByPairCmd(QueryPair pair)
            {
                try
                {
                    StringBuilder cmd = new StringBuilder("SELECT ");
                    var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                    var properties = typeof(MODEL).GetProperties();

                    foreach (var property in properties)
                    {
                        if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                        var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                        //var key = dbRow.Key;
                        //var rowAttr = dbRow.Attribute;
                        switch (dbRow.Attribute)
                        {
                            case RowAttribute.CreatTime:
                            case RowAttribute.NowTime:
                            case RowAttribute.Time:
                                cmd.Append("CONVERT(varchar(20),[" + dbRow.Key + "],120) AS " + dbRow.Key + ",");
                                break;
                            case RowAttribute.Guid:
                                cmd.Append("CONVERT(varchar(36), [" + dbRow.Key + "]) AS " + dbRow.Key + ",");
                                break;
                            default:
                                cmd.Append("[" + dbRow.Key + "],");
                                break;
                        }

                        //if (key.Contains("Utc") ||
                        //    rowAttr.Equals(RowAttribute.CreatTime) ||
                        //    rowAttr.Equals(RowAttribute.NowTime) ||
                        //    rowAttr.Equals(RowAttribute.Time))
                        //{
                        //    cmd.Append("CONVERT(varchar(20),[" + key + "],120) AS " + key + ",");
                        //}
                        //else if (key.Contains("Guid") || rowAttr.Equals(RowAttribute.Guid))
                        //{
                        //    cmd.Append("CONVERT(varchar(36), [" + key + "]) AS " + key + ",");
                        //}
                        //else
                        //{
                        //    cmd.Append("[" + key + "],");
                        //}
                    }
                    cmd.Remove(cmd.Length - 1, 1);
                    var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                    cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                    cmd.Append(pair.getQueryStr());

                    return cmd.ToString();
                }
                catch (Exception)
                {
                    throw new DbModelException();
                }
            }

            [Obsolete]
            public static string DataObjByPairCmd(Dictionary<string, object> pairs)
            {
                try
                {
                    StringBuilder cmd = new StringBuilder("SELECT ");
                    var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                    var properties = typeof(MODEL).GetProperties();
                    foreach (var property in properties)
                    {
                        if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                        var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                        //var key = dbRow.Key;
                        //var rowAttr = dbRow.Attribute;
                        switch (dbRow.Attribute)
                        {
                            case RowAttribute.CreatTime:
                            case RowAttribute.NowTime:
                            case RowAttribute.Time:
                                cmd.Append("CONVERT(varchar(20),[" + dbRow.Key + "],120) AS " + dbRow.Key + ",");
                                break;
                            case RowAttribute.Guid:
                                cmd.Append("CONVERT(varchar(36), [" + dbRow.Key + "]) AS " + dbRow.Key + ",");
                                break;
                            default:
                                cmd.Append("[" + dbRow.Key + "],");
                                break;
                        }

                        //if (key.Contains("Utc") ||
                        //    rowAttr.Equals(RowAttribute.CreatTime) ||
                        //    rowAttr.Equals(RowAttribute.NowTime) ||
                        //    rowAttr.Equals(RowAttribute.Time))
                        //{
                        //    cmd.Append("CONVERT(varchar(20),[" + key + "],120) AS " + key + ",");
                        //}
                        //else if (key.Contains("Guid") || rowAttr.Equals(RowAttribute.Guid))
                        //{
                        //    cmd.Append("CONVERT(varchar(36), [" + key + "]) AS " + key + ",");
                        //}
                        //else
                        //{
                        //    cmd.Append("[" + key + "],");
                        //}
                    }
                    cmd.Remove(cmd.Length - 1, 1);
                    var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                    cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                    for (int i = 0; i < pairs.Count; i++)
                    {
                        if (i < 1)
                            cmd.Append(" WHERE [" + pairs.ElementAt(i).Key + "]='" + pairs.ElementAt(i).Value + "'");
                        else
                            cmd.Append(" AND [" + pairs.ElementAt(i).Key + "]='" + pairs.ElementAt(i).Value + "'");

                    }

                    return cmd.ToString();
                }
                catch (Exception)
                {
                    throw new DbModelException();
                }
            }

            public static string dataById(int id)//use dapper
            {
                try
                {
                    StringBuilder cmd = new StringBuilder("SELECT ");
                    var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                    var properties = typeof(MODEL).GetProperties();
                    var rowPKkey = "Id";
                    foreach (var property in properties)
                    {
                        if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                        var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;

                        //var key = dbRow.Key;
                        //var rowAttr = dbRow.Attribute;
                        if (dbRow.Attribute.Equals(RowAttribute.PrimaryKey))
                            rowPKkey = dbRow.Key;
                        cmd.Append("[" + dbRow.Key + "],");
                    }
                    cmd.Remove(cmd.Length - 1, 1);
                    var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                    cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                    cmd.Append(" WHERE [" + rowPKkey + "]='" + id + "'");
                    return cmd.ToString();
                }
                catch (Exception)
                {
                    throw new DbModelException();
                }
            }

            [Obsolete]
            public static string DataObjByIdCmd(int id)
            {
                try
                {
                    StringBuilder cmd = new StringBuilder("SELECT ");
                    var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;
                    var properties = typeof(MODEL).GetProperties();
                    var rowPKkey = "Id";
                    foreach (var property in properties)
                    {
                        if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                        var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;

                        //var key = dbRow.Key;
                        //var rowAttr = dbRow.Attribute;
                        if (dbRow.Attribute.Equals(RowAttribute.PrimaryKey))
                            rowPKkey = dbRow.Key;

                        switch (dbRow.Attribute)
                        {
                            case RowAttribute.CreatTime:
                            case RowAttribute.NowTime:
                            case RowAttribute.Time:
                                cmd.Append("CONVERT(varchar(20),[" + dbRow.Key + "],120) AS " + dbRow.Key + ",");
                                break;
                            case RowAttribute.Guid:
                                cmd.Append("CONVERT(varchar(36), [" + dbRow.Key + "]) AS " + dbRow.Key + ",");
                                break;
                            default:
                                cmd.Append("[" + dbRow.Key + "],");
                                break;
                        }
                    }
                    cmd.Remove(cmd.Length - 1, 1);
                    var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                    cmd.Append(" FROM " + dbStr + "[" + tableSet.TableName + "] ");
                    cmd.Append(" WHERE [" + rowPKkey + "]='" + id + "'");
                    return cmd.ToString();
                }
                catch (Exception)
                {
                    throw new DbModelException();
                }
            }

            public static string dataById(List<int> list)
            {
                var properties = typeof(MODEL).GetProperties();
                var rowPKkey = "Id";
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;

                    //var key = dbRow.Key;
                    //var rowAttr = dbRow.Attribute;
                    if (!dbRow.Attribute.Equals(RowAttribute.PrimaryKey)) continue;
                    rowPKkey = dbRow.Key;
                }
                return dataByPair(new QueryPair().addQuery(new MultiRow(rowPKkey, list.ConvertAll(new Converter<int, string>(Convert.ToString)))));
            }

            [Obsolete]
            public static string DataObjByIdCmd(List<int> list)
            {
                var properties = typeof(MODEL).GetProperties();
                var rowPKkey = "Id";
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;

                    //var key = dbRow.Key;
                    //var rowAttr = dbRow.Attribute;
                    if (!dbRow.Attribute.Equals(RowAttribute.PrimaryKey)) continue;
                    rowPKkey = dbRow.Key;
                }
                return DataObjByPairCmd(new QueryPair().addQuery(new MultiRow(rowPKkey, list.ConvertAll(new Converter<int, string>(Convert.ToString)))));
            }
        }

        public static class Counter<MODEL> where MODEL : DbOperationModel
        {
            /*
               update TestTable1
               set intTest=(select intTest 
                                       from TestTable1 
                                       where Id=3)+1
                where Id=3
             */
            public static string addById(int id, int offset = 1)
            {
                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;

                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                StringBuilder cmd = new StringBuilder("UPDATE " + dbStr + "[" + tableSet.TableName + "] SET ");
                var properties = typeof(MODEL).GetProperties();
                var rowPKkey = "Id";
                var countKey = new List<string>();
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    if (dbRow.Attribute != RowAttribute.PrimaryKey &&
                        dbRow.Attribute != RowAttribute.Counter) continue;
                    switch (dbRow.Attribute)
                    {
                        case RowAttribute.PrimaryKey:
                            rowPKkey = dbRow.Key;
                            break;
                        case RowAttribute.Counter:
                            countKey.Add(dbRow.Key);
                            break;
                    }
                }
                if (countKey.Count < 1)
                    throw new ArgumentNullException($"This table({tableSet.TableName}) has not any count row!!");

                countKey.ForEach(ck =>
                {
                    var sub = $"select [{ck}] from [{tableSet.TableName}] where [{rowPKkey}]='{id}'";
                    cmd.Append($"[{ck}]=({sub})+{offset},");
                });
                cmd.Remove(cmd.Length - 1, 1);
                cmd.Append($" WHERE [{rowPKkey}]='{id}'");

                return cmd.ToString();
            }

            //public static string addById(MODEL data,int offset=1)
            //{
            //    var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;

            //    var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
            //    StringBuilder cmd = new StringBuilder("UPDATE " + dbStr + "[" + tableSet.TableName + "] SET ");
            //    var properties = typeof(MODEL).GetProperties();
            //    var rowPKkey = "Id";

            //    foreach (var property in properties)
            //    {
            //        if (!property.IsDefined(typeof(DbRowKey), false)) continue;
            //        var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
            //        if (dbRow.Attribute != RowAttribute.PrimaryKey ||
            //            dbRow.Attribute != RowAttribute.Counter) continue;

            //        switch (dbRow.Attribute)
            //        {
            //            case RowAttribute.PrimaryKey:
            //                rowPKkey = dbRow.Key;
            //                break;
            //            case RowAttribute.Counter:
            //                try
            //                {
            //                    var value= Convert.ToInt32(property.GetValue(data, null))+offset;
            //                    cmd.Append("[" + dbRow.Key + "]='" + value + "',");
            //                }
            //                catch
            //                {
            //                    throw new ArgumentException("Counter row type not equal INT");
            //                }                      
            //                break;
            //        }            
            //    }
            //    cmd.Remove(cmd.Length - 1, 1);
            //    cmd.Append(" WHERE [" + rowPKkey + "]='" + data.Id + "'");

            //    return cmd.ToString();
            //}

            public static string addByQuery(QueryPair query, int offset = 1)
            {
                var tableSet = typeof(MODEL).GetCustomAttributes(typeof(DbTableSet), true).FirstOrDefault() as DbTableSet;

                var dbStr = tableSet.IsUseTargetDB ? "[" + tableSet.DbName + "].[dbo]." : "";
                StringBuilder cmd = new StringBuilder("UPDATE " + dbStr + "[" + tableSet.TableName + "] SET ");
                var properties = typeof(MODEL).GetProperties();
                var countKey = new List<string>();
                foreach (var property in properties)
                {
                    if (!property.IsDefined(typeof(DbRowKey), false)) continue;
                    var dbRow = property.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    if (dbRow.Attribute != RowAttribute.Counter) continue;
                    countKey.Add(dbRow.Key);
                }
                if (countKey.Count < 1)
                    throw new ArgumentNullException($"This table({tableSet.TableName}) has not any count row!!");

                countKey.ForEach(ck =>
                {
                    var sub = $"select [{ck}] from [{tableSet.TableName}] {query.getQueryStr()}";
                    cmd.Append($"[{ck}]=({sub})+{offset},");
                });
                cmd.Remove(cmd.Length - 1, 1);
                cmd.Append(query.getQueryStr());

                return cmd.ToString();
            }
        }
        private static Boolean isEmptyStrOrZero(object value)
        {
            try
            {
                var type = value.GetType();

                if (type == typeof(string) || type == typeof(String))
                {
                    return string.IsNullOrEmpty(value.ToString());
                }
                else if (type == typeof(int))
                {
                    return Convert.ToInt32(value) == 0;
                }
                else
                {
                    return value == null;
                }
            }
            catch (Exception e)
            {

            }
            return true;
        }
    }
}

