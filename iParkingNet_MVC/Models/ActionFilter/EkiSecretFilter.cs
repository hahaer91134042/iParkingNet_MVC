using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

/// <summary>
/// EkiSecretFilter 的摘要描述
/// </summary>
public class EkiSecretFilter : BaseFilterAttr
{
    protected override void actionExecuteing(HttpActionContext context)
    {
        CheckSecret();
    }

    private void CheckSecret()
    {
        try
        {
            var secret = getHeaderValue("secret");
            var token = getHeaderValue("token");
            if (secret.Length != 8 || token.isNullOrEmpty())
                throw new ArgumentException();
            if (!SecurityBuilder.CreateEkiSecretHash(secret).Equals(token))
                ResponseError();
        }
        catch (Exception)
        {
            ResponseError();
        }        
    }

   
}