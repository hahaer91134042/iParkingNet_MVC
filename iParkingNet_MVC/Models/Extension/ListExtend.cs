using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Extend 的摘要描述
/// </summary>
public static class ListExtend
{

    public static DbObjList<T> toDbList<T>(this IEnumerable<T> input) where T : BaseDbDAO => new DbObjList<T>(input.toSafeList());
    /// <summary>
    ///   Checks whether all items in the enumerable are same (Uses <see cref="object.Equals(object)" /> to check for equality)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable">The enumerable.</param>
    /// <returns>
    ///   Returns true if there is 0 or 1 item in the enumerable or if all items in the enumerable are same (equal to
    ///   each other) otherwise false.
    /// </returns>
    public static bool AreAllSame<T>(this IEnumerable<T> enumerable)//第一個參數是指定要使用extension的物件類別
    {
        if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));

        using (var enumerator = enumerable.GetEnumerator())
        {
            var toCompare = default(T);
            if (enumerator.MoveNext())
            {
                toCompare = enumerator.Current;
            }

            while (enumerator.MoveNext())
            {
                if (toCompare != null && !toCompare.Equals(enumerator.Current))
                {
                    return false;
                }
            }
        }

        return true;
    }
    //public static bool contains<T>(this IEnumerable<T> list,Func<T,bool> back)
    //{
    //    foreach(var data in list)
    //    {
    //        if (back(data))
    //            return true;
    //    }
    //    return false;
    //    //return (from data in list
    //    //        where back(data)
    //    //        select data).Count() > 0;
    //}

}