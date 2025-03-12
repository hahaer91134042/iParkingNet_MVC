using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eki_FirestoreDB
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DocValue:Attribute
    {
        public string key = "";
        public Type type = typeof(string);
        public DocValue(string k,Type t=null)
        {
            key = k;
            if (t != null)
                type = t;
        }
    }
}