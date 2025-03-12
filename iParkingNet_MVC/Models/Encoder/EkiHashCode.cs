using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// EkiCrypto 的摘要描述
/// </summary>
public class EkiHashCode
{
    public static IHashCodeSet SHA1 = new EkiHash_SHA1();

    private class EkiHash_SHA1 : IHashCodeSet
    {
        public EncryptFormat format() => EncryptFormat.SHA1;
        public string secret() => ApiConfig.JwtSecret;
    }
}