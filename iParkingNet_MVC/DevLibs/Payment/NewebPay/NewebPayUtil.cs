using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// 藍新金流加解密方法
/// </summary>
namespace Eki_NewebPay
{
    public static class NewebPayUtil
    {
        public static string ItemDesc(EkiCheckOut checkOut, Location location)
        {
            return $"車位編號：{location.SerNum}  結帳時間：{checkOut.Date.ToString("yyyy-MM-dd hh:mm:ss")}";
        }
        public static string timeStamp() => ConvertToStamp(DateTime.Now).ToString();
        public static long ConvertToStamp(DateTime input)
        {
            return Convert.ToInt64(input.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }

        public static string EncryptAES256(this INewebPayConfig config, string source)//加密
        {
            //string sSecretKey = PayConfig.藍新.HashKey;
            string sSecretKey = config.hashKey();
            string iv = config.hashIV();
            //string iv = PayConfig.藍新.HashIV;
            byte[] sourceBytes = AddPKCS7Padding(Encoding.UTF8.GetBytes(source), 32);
            var aes = new RijndaelManaged();
            aes.Key = Encoding.UTF8.GetBytes(sSecretKey);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            ICryptoTransform transform = aes.CreateEncryptor();
            return ByteArrayToHex(transform.TransformFinalBlock(sourceBytes, 0, sourceBytes.Length)).ToLower();
        }
        public static string DecryptAES256(this INewebPayConfig config, string encryptData)// 解密
        {
            string sSecretKey = config.hashKey();
            //string sSecretKey = PayConfig.藍新.HashKey;
            string iv = config.hashIV();
            // string iv = PayConfig.藍新.HashIV;
            var encryptBytes = HexStringToByteArray(encryptData.ToUpper());
            var aes = new RijndaelManaged();
            aes.Key = Encoding.UTF8.GetBytes(sSecretKey);
            aes.IV = Encoding.UTF8.GetBytes(iv);
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.None;
            ICryptoTransform transform = aes.CreateDecryptor();
            return Encoding.UTF8.GetString(RemovePKCS7Padding(transform.TransformFinalBlock(encryptBytes, 0, encryptBytes.Length)));
        }

        private static byte[] AddPKCS7Padding(byte[] data, int iBlockSize)
        {
            int iLength = data.Length;
            byte cPadding = (byte)(iBlockSize - (iLength % iBlockSize));
            var output = new byte[iLength + cPadding];
            Buffer.BlockCopy(data, 0, output, 0, iLength);
            for (var i = iLength; i < output.Length; i++)
                output[i] = (byte)cPadding;
            return output;
        }

        private static byte[] RemovePKCS7Padding(byte[] data)
        {
            int iLength = data[data.Length - 1];
            var output = new byte[data.Length - iLength];
            Buffer.BlockCopy(data, 0, output, 0, output.Length);
            return output;
        }
        private static string ByteArrayToHex(byte[] barray)
        {
            char[] c = new char[barray.Length * 2];
            byte b;
            for (int i = 0; i < barray.Length; ++i)
            {
                b = ((byte)(barray[i] >> 4));
                c[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = ((byte)(barray[i] & 0xF));
                c[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
            return new string(c);
        }

        private static byte[] HexStringToByteArray(string hexString)
        {
            int hexStringLength = hexString.Length;
            byte[] b = new byte[hexStringLength / 2];
            for (int i = 0; i < hexStringLength; i += 2)
            {
                int topChar = (hexString[i] > 0x40 ? hexString[i] - 0x37 : hexString[i] - 0x30) << 4;
                int bottomChar = hexString[i + 1] > 0x40 ? hexString[i + 1] - 0x37 : hexString[i + 1] - 0x30;
                b[i / 2] = Convert.ToByte(topChar + bottomChar);
            }
            return b;
        }

        public static string getHashSha256(this INewebPayConfig config, string info)
        {
            var text = $"HashKey={config.hashKey()}&{info}&HashIV={config.hashIV()}";
            //var text = $"HashKey={PayConfig.藍新.HashKey}&{info}&HashIV={PayConfig.藍新.HashIV}";

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
                hashString += String.Format("{0:x2}", x);
            return hashString.ToUpper();
        }
    }
}
