using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;


namespace DevLibs
{
    public class TableParaser
    {

        //public static long timeToStamp(DateTime csTime)
        //{
        //    DateTime javaDate = new DateTime(1970, 1, 1);
        //    TimeSpan javaDiff = csTime.ToUniversalTime() - javaDate.ToUniversalTime();
        //    return (long)javaDiff.TotalMilliseconds;
        //}

        //public static DateTime stampToSysTime(string timeStamp)
        //{
        //    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //    long lTime = long.Parse(timeStamp + "0000000");
        //    TimeSpan toNow = new TimeSpan(lTime);
        //    return dtStart.Add(toNow);
        //}

        public static int GetId(DataTable table)
        {
            try
            {
                DataRow row = table.Rows[0];
                return Convert.ToInt32(row["Id"].ToString());
            }
            catch (Exception) { }
            return -1;
        }

        public static int GetId<T>(DataTable table) where T : DbOperationModel
        {
            try
            {
                var properties = typeof(T).GetProperties();
                foreach (var pro in properties)
                {
                    if (!pro.IsDefined(typeof(DbRowKey), false)) continue;
                    var rowKey = pro.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                    if (rowKey.Attribute != RowAttribute.PrimaryKey) continue;

                    DataRow row = table.Rows[0];
                    return Convert.ToInt32(row[rowKey.Key].ToString());
                }
            }
            catch (Exception) { }
            return -1;
        }

        public static int CountNum(DataTable table)
        {
            try
            {
                return Convert.ToInt32(table.Rows[0]["count"]);
            }
            catch (Exception) { return 0; }

        }

        //從id查找 只會有一筆
        public static T ConvertToObject<T>(DataTable table, T model = null) where T : DbOperationModel//限制給這專案用
        {
            if (table.Rows.Count < 1) return null;
            DataRow row = table.Rows[0];
            return ConvertToObject<T>(row, model);
        }


        public static T ConvertToObject<T>(DataRow row, T model = null) where T : DbOperationModel//限制給這專案用
        {
            var properties = typeof(T).GetProperties();
            var objT = getObjInstance(model);
            foreach (PropertyInfo pro in properties)
            {
                if (!pro.IsDefined(typeof(DbRowKey), false)) continue;

                var key = (pro.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey).Key;
                //var key = pro.GetCustomAttribute<DbRowKey>().Key;//.net 4.5以上

                try
                {
                    if (!Convert.IsDBNull(row[key]))
                        pro.SetValue(objT, Convert.ChangeType(row[key], pro.PropertyType), null);
                    //pro.SetValue(objT, row[key], null);                
                    //pro.SetValue(objT, row[key]);//.net 4.5
                }
                catch (Exception)
                {
                    throw new DataTypeException("Row->" + key + " type can`t convert!");
                }
                //PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                //pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType));
            }
            return objT;
        }


        private static T getObjInstance<T>(T model)
        {
            if (model == null)
                return Activator.CreateInstance<T>();
            return model;
        }



        //抓Row的key來設定值 變數名稱跟row Key不用依樣
        public static List<T> ConvertToListByRowName<T>(DataTable dt)
        {
            //var columnNames = dt.Columns.Cast<DataColumn>()
            //        .Select(c => c.ColumnName)
            //        .ToList();
            if (dt == null)
                return new List<T>();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();

                foreach (var pro in properties)
                {
                    try
                    {
                        if (!pro.IsDefined(typeof(DbRowKey), false)) continue;
                        var dbRow = pro.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                        //DevLibs.ResponseBuilder.getInstance().Append("row key value ", dbRow.Key + ":" + row[dbRow.Key].ToString()).print();
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, row[dbRow.Key] == DBNull.Value ? null : Convert.ChangeType(row[dbRow.Key], pI.PropertyType), null);
                    }
                    catch (Exception)
                    {
                    }
                }
                return objT;
            }).toSafeList();
        }

        //用變數名稱來設定值 row Key根名稱要依樣
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            if (dt == null)
                return new List<T>();
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .ToList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    try
                    {
                        if (columnNames.Contains(pro.Name))
                        {
                            PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                            pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType), null);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                return objT;
            }).toSafeList();
        }

        //抓Row的key來設定值 變數名稱跟row Key不用依樣
        public static IQueryable<T> ConvertQueryByRowName<T>(DataTable dt)
        {
            //var columnNames = dt.Columns.Cast<DataColumn>()
            //        .Select(c => c.ColumnName)
            //        .ToList();
            if (dt == null)
                return new List<T>().AsQueryable();

            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    try
                    {
                        if (!pro.IsDefined(typeof(DbRowKey), false)) continue;
                        var dbRow = pro.GetCustomAttributes(typeof(DbRowKey), true).FirstOrDefault() as DbRowKey;
                        //DevLibs.ResponseBuilder.getInstance().Append("row key value ", dbRow.Key + ":" + row[dbRow.Key].ToString()).print();
                        PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                        pro.SetValue(objT, row[dbRow.Key] == DBNull.Value ? null : Convert.ChangeType(row[dbRow.Key], pI.PropertyType), null);
                    }
                    catch (Exception)
                    {
                    }
                }
                return objT;
            }).AsQueryable();
        }

        //用變數名稱來設定值 row Key根名稱要依樣
        public static IQueryable<T> ConvertQuery<T>(DataTable dt)
        {
            if (dt == null)
                return new List<T>().AsQueryable();
            var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName)
                    .toSafeList();
            var properties = typeof(T).GetProperties();
            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();
                foreach (var pro in properties)
                {
                    try
                    {
                        if (columnNames.Contains(pro.Name))
                        {
                            PropertyInfo pI = objT.GetType().GetProperty(pro.Name);
                            pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : Convert.ChangeType(row[pro.Name], pI.PropertyType), null);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                return objT;
            }).AsQueryable();
        }
    }
}
