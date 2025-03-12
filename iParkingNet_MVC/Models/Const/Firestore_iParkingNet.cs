using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Eki_FirestoreDB;
using DevLibs;

/// <summary>
/// Firestore_PPYP 的摘要描述
/// </summary>
public class Firestore_iParkingNet : IFirestoreConnect
{
    private Firestore_iParkingNet() { }
    public static EkiFirestore db
    {
        get
        {
            if (_db == null)
                start();
            return _db;
        }
    }
    private static EkiFirestore _db;
    public static void start()
    {
        _db = EkiFirestore.start(new Firestore_iParkingNet());
    }

    public override string projectId => "papaya-2256d";
    //public override string projectId => "cps-firebase-5abf9";

    public override string jsonCredit 
    {
        get
        {
            var bytes = iParkingNet_MVC.Properties.Resources.FcmApiFile;
            var jsonStr= Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            //Log.d($"FcmFile->{jsonStr}");
            return jsonStr;
        }
    }

    public override string root => ApiConfig.Firestore_Root;



}