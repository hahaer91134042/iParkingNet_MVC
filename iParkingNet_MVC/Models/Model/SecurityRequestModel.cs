using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// SecurityRequestModel 的摘要描述
/// </summary>
public abstract class SecurityRequestModel<DECODE> : RequestAbstractModel where DECODE:IEncodeSet
{
    public string key { get; set; }
    public string content { get; set; }
    [JsonIgnore]
    public DECODE decode { get;set;}

    public override bool isValid()
    {
        try
        {
            if (key.Length >= 5 && content.Length > 0)
                decode = DecodeContent();

            return true;
        }
        catch (Exception) { }
        return false;
    }

    
    protected DECODE decodeAES()
    {
        return EkiEncoder.AES.decode<DECODE>(content, key);
        //var d = Activator.CreateInstance<DECODE>();
        //var hash = d.creatHash(key);
        //return content.decryptByAES<DECODE>(hash);
    }

    protected DECODE decode3DES()//3DES要棄用了
    {
        var hash = SecurityBuilder.CreateHashCode(EncryptFormat.SHA1, key, ApiConfig.JwtSecret);
        var text = SecurityBuilder.DecryptTextBy3DES(content, hash);
        return text.toObj<DECODE>();
    }

    public abstract DECODE DecodeContent();
}