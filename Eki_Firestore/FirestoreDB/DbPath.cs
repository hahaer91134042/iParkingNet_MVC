using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Eki_FirestoreDB
{
    public class DbPath:List<DbPath.Pair>
    {
        //public const string 
        public string raw = "";
        public string symbol = "%{0}%";
        public DbPath(DbPath o)
        {
            raw = o.raw;
        }
        public DbPath(string t)
        {
            raw = t;
        }
        public DbPath AddMap(Pair p)
        {
            Add(p);
            return this;
        }
        public DbPath AddMap(string k,string v)
        {
            Add(new Pair(k,v));
            return this;
        }

        public override string ToString()
        {
            var path = raw;

            foreach (var p in this)
            {
                //Log.d($"replace key->{p.key} value->{p.value}");
                path = path.Replace($"{string.Format(symbol, p.key)}", p.value);
            }

            //var raw = String.Concat((from p in this
            //                         select $"{p.colFrag.key}/{p.docFrag.key}/"));
            return path;
        }


        public class Pair
        {
            public string key;
            public string value;
            public Pair(string k,string v)
            {
                key = k;
                value =  v;
            }
        }
    }
}