using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Firestore_PPYP.Log;


namespace Eki_FirestoreDB
{
    public class EkiFirestore
    {
        public static EkiFirestore start(IFirestoreConnect conn)
        {
            var builder = new FirestoreClientBuilder
            {
                JsonCredentials = conn.jsonCredit
            };
            var client = FirestoreDb.Create(conn.projectId, builder.Build());
            return new EkiFirestore(conn,client);
        }

        public IFirestoreConnect firestoreConnect;
        public FirestoreDb storeDb;
        public CollectionReference root { get => storeDb.Collection(rootPath); }
        public List<StoreFragment> fragSerial;
        public string rootPath;
        private EkiFirestore(IFirestoreConnect conn,FirestoreDb db) 
        {
            try
            {
                firestoreConnect = conn;
                storeDb = db;
                rootPath = conn.root;
                Log.d($"--- EkiFirestore init  ---");
            }
            catch (Exception e)
            {
                Log.e($"EkiFirestore init error",e);
            }

        }

        public T findDoc<T>(params object[] args)where T : IDocFrag
        {
            var doc = Activator.CreateInstance(typeof(T), args) as T;
            findDoc(doc);
            return doc;
        }

        public T findCol<T>(params object[] args)where T : IColFrag
        {
            var col = Activator.CreateInstance(typeof(T), args) as T;
            findCol(col);
            return col;
        }

        public void findCol(IColFrag colFrag)
        {
            var path = colFrag.path;
            var colNode = storeDb.Collection(path);
            colFrag.setNode(colNode);
        }

        public void findDoc(IDocFrag docFrag)
        {
            var path = docFrag.path;
            //var path = String.Concat((from p in docFrag.docPath()
            //                          select $"{p.colFrag.key}/{p.docFrag.key}/"));
            //path = path.Remove(path.Length - 1);
            //Log.print($"FirestoreDB findDoc path->{path}");
            var docNode = storeDb.Document(path);
            docFrag.setNode(docNode);
            //var values = docNode.GetSnapshotAsync().Result.ToDictionary();
            var snapshot = docNode.GetSnapshotAsync().Result;

            if (!snapshot.Exists)
                return;

            var values = snapshot.ToDictionary();

            //Log.print($"find doc value->{values.toJsonString()}");

            foreach (var prop in docFrag.GetType().GetProperties())
            {
                if (!prop.IsDefined(typeof(DocValue),true))
                    continue;
                var attr = prop.GetCustomAttributes(typeof(DocValue), true).FirstOrDefault() as DocValue;

                if (!values.Any(p=>p.Key==attr.key))
                    continue;

                var pair = values.First(p => p.Key == attr.key);

                prop.SetValue(docFrag, Convert.ChangeType(pair.Value,attr.type));

            }

        }

    }
}