using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eki_FirestoreDB
{
    public static  class IFrag
    {
        public interface Node<T>
        {
            T node { get; }
            void setNode(T node);
        }

        public interface Parent<T>
        {
            T parent();
        }
    }
}