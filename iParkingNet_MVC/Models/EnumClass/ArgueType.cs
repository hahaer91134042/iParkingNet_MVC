using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// CommentType 的摘要描述
/// </summary>
public enum ArgueType
{
    Other=0,//其他
    Report=1,//檢舉
    Illegal_Parking=2,//預約時間外停車
    Bad_Attitude=3,//態度不佳
    Damage_Environment=4,//遺留垃圾/損壞物品
    Dirty_Environment = 5,//環境髒亂
    Fake_Photo =6//虛假照片(車位實際與照片不相符)
    
}