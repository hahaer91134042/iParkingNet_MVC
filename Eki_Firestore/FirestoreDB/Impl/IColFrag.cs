using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;

namespace Eki_FirestoreDB
{
    /// <summary>
    /// 表示 這個片段是Collection
    /// </summary>
    public abstract class IColFrag : StoreFragment,IFrag.Node<CollectionReference>
    {
        public CollectionReference node => _node;
        private CollectionReference _node;
        public void setNode(CollectionReference node) => _node = node;
        public DocumentReference findDoc(string key) => node.Document(key);
        
    }
}