using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevLibs
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbRowKey:Attribute
    {

        private string key;
        private RowAttribute attribute=RowAttribute.NON;
        private DbAction action= DbAction.InsertOnly;
        private Boolean isNullable=true;

        public DbRowKey(string k)
        {
            key = k;
        }

        public DbRowKey(string k,Boolean isNull = true)
        {
            key = k;
            isNullable = isNull;
        }
        public DbRowKey(string k, RowAttribute attr = RowAttribute.NON, Boolean isNull = true)
        {
            key = k;
            attribute = attr;
            isNullable = isNull;
        }
        public DbRowKey(string k,DbAction act=DbAction.InsertOnly,Boolean isNull=true)
        {           
            key = k;
            action = act;
            isNullable = isNull;
        }
        public DbRowKey(string k, RowAttribute attr = RowAttribute.NON, DbAction act = DbAction.InsertOnly, Boolean isNull = true)
        {
            key = k;
            attribute = attr;
            action = act;
            isNullable = isNull;
        }

        public string Key
        {
            get { return key; }
        }
        public RowAttribute Attribute
        {
            get { return attribute; }
        }
        public DbAction Action
        {
            get { return action; }
        }
        public Boolean Nullable
        {
            get { return isNullable; }
        }
    }
    public enum RowAttribute
    {
        NON,//資料型態一般都用變數自身的 以下這些是要特殊處理的
        //目前 時間跟GUID要特別轉換處理
        Time,//不用再insert的時候去輸入 直接給使用者輸入字串DB自己轉換 
        NowTime,//Update的時候使用GETDATE()
        CreatTime,//insert的時候要用GETDATE()
        Guid,
        PrimaryKey,//標示PK的欄位
        Counter//表示 這是個計數器的欄位 可使用 SqlCmd.Counter.Add
    }
    public enum DbAction
    {
        InsertOnly,//Insert=true  Update=false
        Update,//INSERT=true  UPDATE=true
        Static,//Insert=false Update=false
        UpdateOnly//Insert=false UPDATE=true
    }
}