using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ICryptoPwd 的摘要描述
/// </summary>
public interface ICryptoPwd:ICryptoFormat
{
    string newSalt();
    void setSalt(string salt);
    void setCipher(string cipher);
    string salt();
    string cipher();
}