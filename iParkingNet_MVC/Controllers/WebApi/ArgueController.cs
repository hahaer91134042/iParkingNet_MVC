using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

/// <summary>
/// ArgueController 的摘要描述
/// </summary>
[RoutePrefix("api/Argue")]
public class ArgueController:BaseApiController
{
    [HttpPost]
    [Route("Add")]
    [JwtAuthActionFilter]
    public object AddArgur()
    {
        try
        {
            if (!this.formDataContain(RequestFlag.Body.Info))
                throw new ArgumentNullException();

            var request = this.getPostObj<ArgueRequest>(RequestFlag.Body.Info);
            if (!request.isValid())
                throw new InputFormatException();

            var argueManager = ArgueManager.from(getAuthObj());

            var img = this.getPostImg(RequestFlag.Body.Img);

            var success = argueManager.addArgue(request,()=>img);

            if (success)
                return new ArgueResponse(true);
            else
                return ResponseError(EkiErrorCode.E023);
        }
        catch (ArgumentNullException)
        {
            return ResponseError(EkiErrorCode.E003);
        }
        catch (AddErrorException)
        {
            return ResponseError(EkiErrorCode.E023);
        }
        catch (InputFormatException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (Exception) { }
        return ResponseError();
    }

    public class ArgueResponse : ResponseAbstractModel
    {
        public ArgueResponse(bool successful) : base(successful)
        {
        }
    }

}