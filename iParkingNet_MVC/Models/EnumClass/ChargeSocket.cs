using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ChargeSocket 的摘要描述
/// </summary>
public enum ChargeSocket
{
    Abort=-1,//只有使用了v2的才會出現這flag
    NONE =0,//之後 應該也沒用了 
    ALL=1,//之後應該沒用了
    Type2_CCS=2,//CCS2 DC IEC62196
    CHAdeMO=3,
    Tesla=4,
    Type1_CCS=5,//DC SAEJ1772
    GB_T_20234=6,
    SAEJ1772=7,//AC 
    CNS15511=8,//xx這 沒用
    IEC62196=9,//AC     

    e_Moving=10,//機車用的
    Gogoro=11,//機車用的
    ionex=12,//機車用
    Home=13//家用插頭
}