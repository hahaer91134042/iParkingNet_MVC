using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ICrypto 的摘要描述
/// </summary>
public interface IHashCodeSet:ICryptoFormat
{
   // EncryptFormat format();
    string secret();
}