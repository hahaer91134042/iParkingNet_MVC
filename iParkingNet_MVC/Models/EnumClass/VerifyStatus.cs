using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// VerifyStatus 的摘要描述
/// 紀錄該筆審核目前的狀態
/// </summary>
public enum VerifyStatus
{
    Processing=0,//審核中
    Pass=1,//審核完成(通過)
    UnPass=2//取消(未通過)
}