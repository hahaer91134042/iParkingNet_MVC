using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EkiEncoder 的摘要描述
/// </summary>
public class EkiEncoder
{
    public static IEncoder AES = new EkiAESencoder();

    private class EkiAESencoder : IEncoder
    {
        public override T decode<T>(string cipher, string key = "")
        {
            var obj = Activator.CreateInstance<T>();
            var hash = obj.creatHash(key);
            return cipher.decryptByAES<T>(hash);
        }

        public override CryptoContent encode<T>(T data)
        {
            var key = SecurityBuilder.CreateSaltKey();
            var hash = data.creatHash(key);
            return new CryptoContent()
            {
                publicKey = key,
                cipher = data.encryptByAES(hash)
            };
        }
    }


}