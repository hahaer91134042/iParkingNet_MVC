using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Newtonsoft.Json;
using Firestore_PPYP.Log;

namespace Eki_FirestoreDB
{
    /// <summary>
    /// 表示 這個片段是Document
    /// </summary>
    public abstract class IDocFrag : StoreFragment,IFrag.Node<DocumentReference>
    {
        public DocumentReference node => _node;
        private DocumentReference _node;
        public void setNode(DocumentReference node) => _node = node;
        public CollectionReference findCol(string key) => node.Collection(key);
        //public abstract DbPath docPath();
        //public override string path => docPath().ToString();

        /// <summary>
        /// 這邊提供另外一種資料模式
        /// 可使用外部的資料來當內容
        /// 需使用到FirestoreData  FirestoreProperty
        /// 來標記該資料物件
        /// </summary>
        /// <returns></returns>
        public T data<T>()
        {
            var snapshot = node.GetSnapshotAsync();

            if (!snapshot.Result.Exists)
                return default(T);

            return snapshot.Result.ConvertTo<T>();
        }

        /// <summary>
        /// 這邊提供另外一種紀錄模式
        /// 可使用外部的資料來當內容
        /// 需使用到FirestoreData  FirestoreProperty
        /// 來標記該資料物件
        /// </summary>
        /// <param name="data">class need FirestoreData or Dictionary<string, object>()</param>
        /// <param name="mode">default= SaveMode.Overwrite</param>
        /// <returns></returns>
        public bool save(object data,SaveMode mode = null)
        {
            try
            {

                node.SetAsync(data, mode == null ? SaveMode.Overwrite.option : mode.option);
                return true;
            }
            catch (Exception e)
            {
                Log.e($"Firestore doc {GetType().Name} save error", e);
                return false;
            }
        }

        public bool save(SaveMode mode= null)
        {
            try
            {
                if (node == null)
                    throw new ArgumentNullException("Doc node not be null");


                var values = new Dictionary<string, object>();

                foreach (var prop in GetType().GetProperties())
                {
                    if (!prop.IsDefined(typeof(DocValue), true))
                        continue;
                    var attr = prop.GetCustomAttributes(typeof(DocValue), true).FirstOrDefault() as DocValue;

                    values.Add(attr.key, prop.GetValue(this));
                }

                //Log.d($"Firestore save path->{path} data->{JsonConvert.SerializeObject(values)}");
                //預設記錄模式 OverWrite
                node.SetAsync(values,mode==null?SaveMode.Overwrite.option:mode.option);
                return true;
            }
            catch (Exception e)
            {
                Log.e($"Firestore doc {GetType().Name} save error", e);
                return false;
            }
        }
    }
}