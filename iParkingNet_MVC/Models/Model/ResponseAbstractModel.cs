using System;
using System.ComponentModel;
using System.Reflection;
using System.Web;

public abstract class ResponseAbstractModel : ApiAbstractModel
{
    private EkiErrorCode code;

    public ResponseAbstractModel(Boolean successful)
    {
        success = successful;
        if (!successful)
        {

            errorCode = EkiErrorCode.E004.ToString();
            //message = errorCode.GetType().GetCustomAttribute<DescriptionAttribute>().Description;
        }
        else
        {
            //message = "success";
            errorCode = EkiErrorCode.E000.ToString();
        }
    }
    public Boolean success { get; set; }
    //error message use 可以預設 也可以後來在自訂 看constructor
    public string message { get { return GetDescription(code); } }
    public string errorCode { get { return code.ToString(); } set { code = (EkiErrorCode)Enum.Parse(typeof(EkiErrorCode), value); } }

    //private string GetDescription(Enum value, Boolean nameInstead)
    //{
    //    Type type = value.GetType();
    //    string name = Enum.GetName(type, value);
    //    if (name == null)
    //    {
    //        return null;
    //    }

    //    FieldInfo field = type.GetField(name);
    //    DescriptionAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

    //    if (attribute == null && nameInstead == true)
    //    {
    //        return name;
    //    }
    //    return attribute == null ? null : attribute.Description;
    //}

    private string GetDescription(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());
        DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }

    public ResponseAbstractModel setErrorCode(EkiErrorCode code)
    {
        errorCode = code.ToString();
        //message = errorCode.GetType().GetCustomAttribute<DescriptionAttribute>().Description;
        return this;
    }



}



public enum EkiErrorCode : int
{
    [Description("success")]
    E000 = 0,//正常

    [Description("參數輸入錯誤")]
    E001 = 1,//輸入錯誤

    [Description("超出時間")]
    E002 = 2,//超出時間

    [Description("無資料")]
    E003 = 3,//無資料

    [Description("未知錯誤")]
    E004 = 4,//未知錯誤

    [Description("沒有這個帳號")]
    E005 = 5,

    [Description("資料格式錯誤")]
    E006 = 6,

    [Description("沒有輸入地址")]
    E007 = 7,

    [Description("信箱已存在")]
    E008 = 8,

    [Description("手機號碼已存在")]
    E009 = 9,

    [Description("Token已過期")]
    E010 = 10,

    [Description("Token驗證錯誤")]
    E011 = 11,

    [Description("帳號或密碼錯誤")]
    E012 = 12,

    [Description("檔案格式錯誤")]
    E013 = 13,

    [Description("無檔案")]
    E014 = 14,

    [Description("手機號碼錯誤")]
    E015 = 15,

    [Description("超出數量")]
    E016 = 16,

    [Description("權限不足")]
    E017 = 17,

    [Description("預約失敗")]
    E018 = 18,

    [Description("次數不足")]
    E019 = 19,

    [Description("帳單未付款")]
    E020 = 20,

    [Description("時間重疊")]
    E021 = 21,

    [Description("權限錯誤")]
    E022 = 22,

    [Description("加入錯誤")]
    E023=23,

    [Description("帳號以停權")]
    E024=24
}
