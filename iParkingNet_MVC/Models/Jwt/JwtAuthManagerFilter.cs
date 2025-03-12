using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Jose;
/// <summary>
/// 驗證token是否為地主
/// </summary>
public class JwtAuthManagerFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        // TODO: key應該移至config
        //var secret = "!qaz2WSX#edc";


        if (actionContext.Request.Headers.Authorization == null || 
            actionContext.Request.Headers.Authorization.Scheme != RequestFlag.Header.EkiScheme)
        {
            setErrorResponse(actionContext, EkiErrorCode.E011);
        }
        else
        {
            try
            {
                //var jwtObject = Jose.JWT.Decode<JwtAuthObject>(
                //    actionContext.Request.Headers.Authorization.Parameter,
                //    Encoding.UTF8.GetBytes(secret),
                //    JwsAlgorithm.HS256);
                var jwtObject = JwtBuilder.GetDecoder()
                            .setToken(actionContext.Request.Headers.Authorization.Parameter)
                            .decode();


                //if (( jwtObject.exp.toDateTime()-DateTime.Now).TotalDays < 0)
                //{
                //    setErrorResponse(actionContext, EkiErrorCode.E010);
                //}
                var member = jwtObject.getMember();

                if (!member.beEnable)
                    setErrorResponse(actionContext, EkiErrorCode.E024);
                if (!member.beManager)
                    setErrorResponse(actionContext, EkiErrorCode.E017);

                //DateTime t1 = new DateTime("2010-10-01");
                //DateTime t2 = new DateTime("2010-10-02");
                //int result = DateTime.Compare(t1, t2);
                //result 值為 -1
                //以代表 t1 時間 小於 t2 時間
            }
            catch (Exception ex)
            {
                setErrorResponse(actionContext, EkiErrorCode.E011);
                //throw new JwtException(EkiErrorCode.E011);
            }
        }

        base.OnActionExecuting(actionContext);
    }

    //private static void setErrorResponse(HttpActionContext actionContext, string message)
    //{
    //    var response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, message);
    //    response.Content = new ObjectContent();
    //    actionContext.Response = response;
    //}
    private static void setErrorResponse(HttpActionContext actionContext, EkiErrorCode errorCode)
    {
        var response = actionContext.Request.CreateResponse<JwtErrorResponse>(HttpStatusCode.Unauthorized,new JwtErrorResponse(errorCode));
        actionContext.Response = response;

    }

    private class JwtErrorResponse : ResponseAbstractModel
    {
        public JwtErrorResponse(EkiErrorCode code) : base(false)
        {
            setErrorCode(code);
        }
    }
}
