using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;

namespace Eki_FirestoreDB
{
    public class SaveMode
    {
        public static SaveMode Overwrite = new SaveMode(SetOptions.Overwrite);
        public static SaveMode MergeAll = new SaveMode(SetOptions.MergeAll);

        public SetOptions option;
        public SaveMode(SetOptions opt)
        {
            option = opt;
        }
    }
}
