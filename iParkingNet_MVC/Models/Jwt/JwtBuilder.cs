using Jose;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// JwtBuilder 的摘要描述
/// </summary>
#region ---JwtBuilder--- 需使用Jose-jwt
public class JwtBuilder
{
    //public const string defaultSecret = "!qaz2WSX#edc";

    public static Encoder GetEncoder(string secret = ApiConfig.JwtSecret, JwsAlgorithm algorithm = JwsAlgorithm.HS256)
    {
        return new Encoder(secret, algorithm);
    }
    public static Decoder GetDecoder(string secrety = ApiConfig.JwtSecret, JwsAlgorithm algorithm = JwsAlgorithm.HS256)
    {
        return new Decoder(secrety, algorithm);
    }

    public class Encoder
    {
        private JwtAuthObject payload = new JwtAuthObject();
        private string dotReplaceTo = ".";
        private string secret;
        private JwsAlgorithm algorithm;

        public Encoder(string secret, JwsAlgorithm algorithm)
        {
            this.secret = secret;
            this.algorithm = algorithm;
        }
        public Encoder setUser(string user)
        {
            payload.user = user;
            return this;
        }
        public Encoder setExpDate(DateTime time)
        {
            payload.exp = time.ToString(ApiConfig.DateFormat);
            return this;
        }
        public Encoder setSub(string sub)
        {
            payload.sub = sub;
            return this;
        }
        public Encoder dotReplace(string to)
        {
            dotReplaceTo = to;
            return this;
        }

        public string encode()
        {
            string token = Jose.JWT.Encode(payload, Encoding.UTF8.GetBytes(secret), algorithm);
            return token.Replace(".", dotReplaceTo);
        }
    }

    public class Decoder
    {
        private string secret;
        private JwsAlgorithm algorithm;
        private string replace = ".";
        private string tokenStr;
        
        public Decoder(string secret, JwsAlgorithm algorithm)
        {
            this.secret = secret;
            this.algorithm = algorithm;
        }
        public Decoder ReplaceToDot(string whichTo)
        {
            replace = whichTo;
            return this;
        }

        public Decoder setToken(string token)
        {
            tokenStr = token;
            return this;
        }
        public JwtAuthObject decode()
        {
            return Jose.JWT.Decode<JwtAuthObject>(
                tokenStr.Replace(replace, "."),
                Encoding.UTF8.GetBytes(secret),
                algorithm);
        }
    }
}
#endregion