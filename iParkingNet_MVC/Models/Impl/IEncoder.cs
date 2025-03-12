using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ISecret 的摘要描述
/// </summary>
public abstract class IEncoder
{
    public abstract T decode<T>(string cipher,string key="")where T:IEncodeSet;
    public abstract CryptoContent encode<T>(T data)where T:IEncodeSet;

    public class CryptoContent
    {
        public string publicKey { get; set; }
        public string cipher { get; set; }
    }
}