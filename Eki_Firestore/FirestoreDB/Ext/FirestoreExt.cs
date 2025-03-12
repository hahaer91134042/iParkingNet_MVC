using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eki_FirestoreDB
{
    public static class FirestoreExt
    {
        public static Timestamp toUtcStamp(this DateTime input,DateTimeKind kind=DateTimeKind.Utc)
        {
            var time = new DateTime(input.Year, input.Month, input.Day,
                input.Hour, input.Minute, input.Second, input.Millisecond, kind);
            return Timestamp.FromDateTime(time);
        }

    }
}
