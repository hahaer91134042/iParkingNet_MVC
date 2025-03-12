using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Eki_FirestoreDB;

/// <summary>
/// Path_PPYP 的摘要描述
/// </summary>
public class Path_PPYP
{
    public static DbPath root = new DbPath(ApiConfig.Firestore_Root);
    public static DbPath doc_location = new DbPath($"{root.raw}/location");
    public static DbPath col_locSer = new DbPath($"{doc_location.raw}/%{Flag.locSerial}%");

    

    public class Flag
    {
        public const string locSerial = "locSerial";
    }
}