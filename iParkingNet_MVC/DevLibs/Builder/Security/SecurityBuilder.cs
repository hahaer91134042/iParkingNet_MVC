using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

/// <summary>
/// SecurityBuilder 的摘要描述
/// </summary>
#region --SecurityBuilder--
public class SecurityBuilder
{
    public const string defaultSecret = "!qaz2WSX#edc";

    public static string CreateEkiSecretHash(string secret, EncryptFormat format = EncryptFormat.SHA256)
    {
        string combine = String.Concat("Eki", secret, defaultSecret);
        var algorithm = HashAlgorithm.Create(format.ToString());

        if (algorithm == null)
            throw new ArgumentException("Unrecognized hash name");

        var hashByteArray = algorithm.ComputeHash(Encoding.UTF8.GetBytes(combine));
        return BitConverter.ToString(hashByteArray).Replace("-", "");
    }

    /// <summary>
    /// Create salt key 密碼salt size=5
    /// </summary>
    /// <param name="size">Key size</param>
    /// <returns>Salt key</returns>
    public static string CreateSaltKey(int size = 5)
    {
        // Generate a cryptographic random number
        var rng = new RNGCryptoServiceProvider();
        var buff = new byte[size];
        rng.GetBytes(buff);
        // Return a Base64 string representation of the random number
        return Convert.ToBase64String(buff);
    }
    /// <summary>
    /// Create a password hash 密碼EncryptFormat用SHA256
    /// </summary>
    /// <param name="password">password</param>
    /// <param name="saltkey">Salk key</param>
    /// <param name="format">Password format (hash algorithm)</param>
    /// <returns>Password hash</returns>
    public static string CreatePasswordHash(string saltkey, string password, EncryptFormat format = EncryptFormat.SHA1)
    {

        string saltAndPassword = String.Concat(saltkey, password);

        //return FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPassword, passwordFormat);
        var algorithm = HashAlgorithm.Create(format.ToString());
        if (algorithm == null)
            throw new ArgumentException("Unrecognized hash name");

        var hashByteArray = algorithm.ComputeHash(Encoding.UTF8.GetBytes(saltAndPassword));
        return BitConverter.ToString(hashByteArray).Replace("-", "");
    }

    public static string CreateHashCode(EncryptFormat format, params string[] values)
    {
        var text = string.Concat(values);
        var algorithm = HashAlgorithm.Create(format.ToString());
        if (algorithm == null)
            throw new ArgumentException("Unrecognized hash name");
        var hashByteArray = algorithm.ComputeHash(Encoding.UTF8.GetBytes(text));
        return BitConverter.ToString(hashByteArray).Replace("-", "");
    }

    /// <summary>
    /// Encrypt text
    /// </summary>
    /// <param name="plainText">Text to encrypt</param>
    /// <param name="encryptionPrivateKey">Encryption private key</param>
    /// <returns>Encrypted text</returns>
    public virtual string EncryptTextBy3DES(string plainText, string encryptionPrivateKey)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        //if (String.IsNullOrEmpty(encryptionPrivateKey))
        //    encryptionPrivateKey = _securitySettings.EncryptionKey;
        using (var tDESalg = new TripleDESCryptoServiceProvider())
        {
            tDESalg.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            //tDESalg.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));
            tDESalg.Mode = CipherMode.CBC;

            return EncryptTextToBase64(plainText, tDESalg.CreateEncryptor());
        }
    }

    //public  string Encrypt3DES(string toEncrypt, bool useHashing)
    //{
    //    byte[] keyArray;
    //    byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

    //    //System.Configuration.AppSettingsReader settingsReader =
    //    //                                    new AppSettingsReader();
    //    //// Get the key from config file

    //    //string key = (string)settingsReader.GetValue("SecurityKey",
    //    //                                                 typeof(String));
    //    //System.Windows.Forms.MessageBox.Show(key);
    //    //If hashing use get hashcode regards to your key
    //    var key = ApiConfig.JwtSecret;

    //    if (useHashing)
    //    {
    //        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
    //        keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
    //        //Always release the resources and flush data
    //        // of the Cryptographic service provide. Best Practice

    //        hashmd5.Clear();
    //    }
    //    else
    //        keyArray = UTF8Encoding.UTF8.GetBytes(key);

    //    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
    //    //set the secret key for the tripleDES algorithm
    //    tdes.Key = keyArray;
    //    //mode of operation. there are other 4 modes.
    //    //We choose ECB(Electronic code Book)
    //    tdes.Mode = CipherMode.ECB;
    //    //padding mode(if any extra byte added)

    //    tdes.Padding = PaddingMode.PKCS7;

    //    ICryptoTransform cTransform = tdes.CreateEncryptor();
    //    //transform the specified region of bytes array to resultArray
    //    byte[] resultArray =
    //      cTransform.TransformFinalBlock(toEncryptArray, 0,
    //      toEncryptArray.Length);
    //    //Release resources held by TripleDes Encryptor
    //    tdes.Clear();
    //    //Return the encrypted data into unreadable string format
    //    return Convert.ToBase64String(resultArray, 0, resultArray.Length);
    //}
    //public  string Decrypt3DES(string cipherString, bool useHashing)
    //{
    //    byte[] keyArray;
    //    //get the byte code of the string

    //    byte[] toEncryptArray = Convert.FromBase64String(cipherString);

    //    //System.Configuration.AppSettingsReader settingsReader =
    //    //                                    new AppSettingsReader();
    //    //Get your key from config file to open the lock!
    //    //string key = (string)settingsReader.GetValue("SecurityKey",
    //    //                                             typeof(String));
    //    var key=ApiConfig.JwtSecret;

    //    if (useHashing)
    //    {
    //        //if hashing was used get the hash code with regards to your key
    //        MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
    //        keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
    //        //release any resource held by the MD5CryptoServiceProvider

    //        hashmd5.Clear();
    //    }
    //    else
    //    {
    //        //if hashing was not implemented get the byte code of the key
    //        keyArray = UTF8Encoding.UTF8.GetBytes(key);
    //    }

    //    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
    //    //set the secret key for the tripleDES algorithm
    //    tdes.Key = keyArray;
    //    //mode of operation. there are other 4 modes. 
    //    //We choose ECB(Electronic code Book)

    //    tdes.Mode = CipherMode.ECB;
    //    //padding mode(if any extra byte added)
    //    tdes.Padding = PaddingMode.PKCS7;

    //    ICryptoTransform cTransform = tdes.CreateDecryptor();
    //    byte[] resultArray = cTransform.TransformFinalBlock(
    //                         toEncryptArray, 0, toEncryptArray.Length);
    //    //Release resources held by TripleDes Encryptor                
    //    tdes.Clear();
    //    //return the Clear decrypted TEXT
    //    return UTF8Encoding.UTF8.GetString(resultArray);
    //}

    /// <summary>
    /// Decrypt text
    /// </summary>
    /// <param name="cipherText">Text to decrypt</param>
    /// <param name="encryptionPrivateKey">Encryption private key</param>
    /// <returns>Decrypted text</returns>
    public static string DecryptTextBy3DES(string cipherText, string encryptionPrivateKey)
    {
        if (String.IsNullOrEmpty(cipherText))
            return cipherText;

        //if (String.IsNullOrEmpty(encryptionPrivateKey))
        //    encryptionPrivateKey = _securitySettings.EncryptionKey;
        using (var tDESalg = new TripleDESCryptoServiceProvider())
        {
            tDESalg.Key = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(0, 16));
            //tDESalg.IV = new ASCIIEncoding().GetBytes(encryptionPrivateKey.Substring(8, 8));
            tDESalg.Mode = CipherMode.CBC;

            return DecryptTextFromBase64(cipherText, tDESalg.CreateDecryptor());
        }
    }

    private static string EncryptTextToBase64(string data, ICryptoTransform transform)
    {
        byte[] toEncryptArray = Encoding.UTF8.GetBytes(data);
        byte[] resultArray =
          transform.TransformFinalBlock(toEncryptArray, 0,
          toEncryptArray.Length);
        return Convert.ToBase64String(resultArray);
    }
    //private byte[] EncryptTextToMemory(string data, byte[] key, byte[] iv)
    //{
    //    using (var ms = new MemoryStream())
    //    {
    //        using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateEncryptor(key, iv), CryptoStreamMode.Write))
    //        {
    //            byte[] toEncrypt = new UnicodeEncoding().GetBytes(data);
    //            cs.Write(toEncrypt, 0, toEncrypt.Length);
    //            cs.FlushFinalBlock();
    //        }

    //        return ms.ToArray();
    //    }
    //}

    private static string DecryptTextFromBase64(string cipher, ICryptoTransform transform)
    {
        byte[] data = Convert.FromBase64String(cipher);
        byte[] resultArray = transform.TransformFinalBlock(
                             data, 0, data.Length);
        return Encoding.UTF8.GetString(resultArray);
    }
    //private string DecryptTextFromMemory(byte[] data, byte[] key, byte[] iv)
    //{
    //    using (var ms = new MemoryStream(data))
    //    {
    //        using (var cs = new CryptoStream(ms, new TripleDESCryptoServiceProvider().CreateDecryptor(key, iv), CryptoStreamMode.Read))
    //        {
    //            var sr = new StreamReader(cs, new UnicodeEncoding());
    //            return sr.ReadLine();
    //        }
    //    }
    //}

    /// <summary>
    /// 字串加密(非對稱式)
    /// </summary>
    /// <param name="Source">加密前字串</param>
    /// <param name="CryptoKey">加密金鑰</param>
    /// <returns>加密後字串</returns>
    public static string EncryptTextByAES(string Source, string CryptoKey)
    {
        try
        {
            using (var md5 = new MD5CryptoServiceProvider())
            using (var aes = new AesCryptoServiceProvider())
            {

                //SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                //byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                //byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                aes.Key = md5.ComputeHash(Encoding.ASCII.GetBytes(CryptoKey.Substring(0, 16)));
                //aes.IV = sha256.ComputeHash(Encoding.ASCII.GetBytes(CryptoKey.Substring(8, 8)));
                aes.IV = md5.ComputeHash(Encoding.ASCII.GetBytes(CryptoKey.Substring(8, 8)));
                aes.Mode = CipherMode.CBC;

                //byte[] dataByteArray = Encoding.UTF8.GetBytes(SourceStr);

                return EncryptTextToBase64(Source, aes.CreateEncryptor());
            }
        }
        catch (Exception e)
        {
            //throw e;
        }
        return "";
    }

    public static string DecryptTextByAES(string Cipher, string CryptoKey)
    {
        try
        {
            using (var md5 = new MD5CryptoServiceProvider())
            using (var aes = new AesCryptoServiceProvider())
            {
                //MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                //SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();
                //byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                //byte[] iv = md5.ComputeHash(Encoding.UTF8.GetBytes(CryptoKey));
                aes.Key = md5.ComputeHash(Encoding.ASCII.GetBytes(CryptoKey.Substring(0, 16)));
                aes.IV = md5.ComputeHash(Encoding.ASCII.GetBytes(CryptoKey.Substring(8, 8)));
                aes.Mode = CipherMode.CBC;

                return DecryptTextFromBase64(Cipher, aes.CreateDecryptor());
            }
        }
        catch (Exception e)
        {
            throw e;
        }
    }

}
#endregion