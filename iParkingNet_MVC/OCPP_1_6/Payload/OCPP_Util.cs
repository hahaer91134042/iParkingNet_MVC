using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// OCPP_Time 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public class OCPP_Util
    {
        public static string nowTime() => DateTime.Now.dateToCpStr();
        public static string creatUid(int length=OCPP_Config.UidLength) => RandomUtil.GetRandomString(length, RandomString.All);

    }
}