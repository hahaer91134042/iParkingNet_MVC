using Fleck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevLibs;
using System.Globalization;

/// <summary>
/// OCPP_Ext 的摘要描述
/// </summary>
namespace OCPP_1_6
{
    public static class OCPP_Ext
    {
        #region IEnumConvert
        public static T status<T>(this IEnumConvert obj) => obj.getEnum<T>(StatusEnum.flag);
        public static bool setStatus<T>(this IEnumConvert obj, T value) => obj.setEnum(value, StatusEnum.flag);

        public static T unit<T>(this IEnumConvert obj) => obj.getEnum<T>(UnitEnum.flag);
        public static bool setUnit<T>(this IEnumConvert obj, T value) => obj.setEnum(value, UnitEnum.flag);
        #endregion

        public static T getEnum<T>(this object obj,string key = "")
        {
            var props = obj.GetType().GetProperties();
            var attrType = typeof(ConvertEnum);
            foreach(var info in props)
            {
                if (!info.IsDefined(attrType, true)) continue;

                var convert = info.GetCustomAttributes(attrType, true).FirstOrDefault() as ConvertEnum;
                //key不等於設定好的key跳過 
                if (!string.IsNullOrEmpty(key) && convert.key != key) continue;
                
                if (!convert.type.IsEnum) continue;
                //要輸出的Enum 型態檢查
                if (!typeof(T).Equals(convert.type)) continue;

                var value = Convert.ToString(info.GetValue(obj));
                return (T)Enum.Parse(typeof(T), value, false);
            }

            return default(T);
        }

        public static bool setEnum<T>(this object obj,T value,string key="")
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException("Input value must be Enum Type");
            try
            {

                var props = obj.GetType().GetProperties();
                var attrType = typeof(ConvertEnum);
                foreach (var info in props)
                {
                    if (!info.IsDefined(attrType, true)) continue;

                    var convert = info.GetCustomAttributes(attrType, true).FirstOrDefault() as ConvertEnum;
                    //key不等於設定好的key跳過 
                    if (!string.IsNullOrEmpty(key) && convert.key != key) continue;

                    if (!convert.type.IsEnum) continue;
                    //要輸入的Enum 型態檢查
                    if (!typeof(T).Equals(convert.type)) continue;
                    //必須轉換成string
                    if (!info.PropertyType.Equals(typeof(string))) throw new ArgumentException("Property type must string!");

                    info.SetValue(obj, Convert.ChangeType(value.ToString(), info.PropertyType));
                    return true;
                }

                return false;
            }
            catch(Exception e)
            {

            }
            return false;
        }

        public static DateTime getTime(this IOCPP_Time input)
            => input.timeStr().toCpTime();        
        public static string dateToCpStr(this DateTime input)
            =>input.ToString(OCPP_Config.OCPP_TimeFormate);
        public static DateTime toCpTime(this string input)
            =>DateTime.ParseExact(input, OCPP_Config.OCPP_TimeFormate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        public static void SendOCPP(this IWebSocketConnection socket, OCPP_Msg msg)
        {
            //Log.print($"Send OCPP msg->{msg.toJsonString()}");
            socket.Send(msg.toJsonString());
        }
        public static void SendOCPP(this IWebSocketConnection socket,IMsgBuilder<OCPP_Msg> builder)
        {
            socket.SendOCPP(builder.build());
        }
    }
}
