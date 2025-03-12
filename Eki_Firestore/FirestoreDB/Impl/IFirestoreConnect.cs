using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eki_FirestoreDB
{
    public abstract class IFirestoreConnect
    {
        public abstract string projectId { get; }
        public abstract string jsonCredit { get; }
        public abstract string root { get; }
    }
}