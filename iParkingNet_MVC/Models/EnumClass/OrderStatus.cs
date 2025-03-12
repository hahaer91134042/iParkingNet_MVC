using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OrderStatus 的摘要描述
/// </summary>
public enum OrderStatus
{

    /// <summary>
    /// 已預約
    /// </summary>
    Reserved = 0,
    /// <summary>
    /// 使用中 目前技術上用不到
    /// </summary>
    InUsing = 1,
    /// <summary>
    /// 待結帳
    /// </summary>
    BeSettle = 2,             //待結帳 EkiCheckout已經有資料
    /// <summary>
    /// 已結帳
    /// </summary>
    CheckOut = 3,             //已結帳 EkiCheckout已經有資料 藍新已經付過款
    /// <summary>
    /// 已取消
    /// </summary>
    Cancel = 4,
    /// <summary>
    /// 爭議訂單
    /// </summary>
    Disputed = 5,
    /// <summary>
    /// 付款過程出現問題
    /// </summary>
    PayError = 6,
    /// <summary>
    /// 地主因為關閉開放時間而取消
    /// </summary>
    CancelByManager = 7
}