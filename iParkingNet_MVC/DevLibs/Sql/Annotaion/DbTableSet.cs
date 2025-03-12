using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DevLibs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableSet : Attribute
    {
        private string table;
        private string db;
        private bool isUseTargetDB;
        public DbTableSet(string tableName)
        {
            table = tableName;
            db = "";
            isUseTargetDB = false;
        }
        public DbTableSet(string tableName,string dbName)
        {
            table = tableName;
            db = dbName;
            isUseTargetDB = true;
        }

        public string TableName
        {
            get { return table; }
        }
        public string DbName
        {
            get { return db; }
        }
        public bool IsUseTargetDB
        {
            get { return isUseTargetDB; }
        }
    }
}