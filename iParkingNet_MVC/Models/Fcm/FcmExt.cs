using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// FcmExt 的摘要描述
/// </summary>
public static class FcmExt
{
    public static void sendTo(this IFcmMsg msg, IFcmSet set) 
    {
        using (var manager = FcmManager.New())
        {
            manager.SendTo(set, msg);
        }
    }
    public static void sendTo(this IFcmMsg msg,IEnumerable<IFcmSet> list)
    {
        using (var manager = FcmManager.New())
        {
            foreach(var set in list)
            {
                manager.SendTo(set, msg);
            }
        }
    }
 }