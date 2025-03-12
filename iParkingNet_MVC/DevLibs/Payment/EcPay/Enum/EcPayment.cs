using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EcPayment 的摘要描述
/// </summary>
public enum EcPayment
{
    Credit,//信用卡
    WebATM,//網路 ATM
    ATM,//自動櫃員機
    CVS,//超商代碼
    BARCODE,//超商條碼
    ALL//不指定付款方式，由綠界顯示付款方式選擇頁面
}