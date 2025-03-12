using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

/// <summary>
/// RatingController 的摘要描述
/// </summary>
[RoutePrefix("api/Rating")]
public class RatingController:BaseApiController
{


    [HttpPost]
    [Route("Location")]
    [JwtAuthActionFilter]
    public object RatingLocation(RatingRequest request)
    {
        //這是車主對地點的評分
        try
        {
            if (!request.isValid())
                throw new ArgumentException();
            var auth = getAuthObj();

            var ratingManager = RatingManager.from(auth);
            var success = ratingManager.addLocationRating(request);

            if (success)
                return new RatingResponse(true);
            else
                return new RatingResponse(false).setErrorCode(EkiErrorCode.E023);
        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (AddErrorException)
        {
            return ResponseError(EkiErrorCode.E023);
        }        
        catch (Exception) { }
        return ResponseError();
    }

    [HttpPost]
    [Route("User")]
    [JwtAuthActionFilter]
    public object RatingUser(RatingRequest request)
    {   //地主評價車主
        try
        {
            if (!request.isValid())
                throw new ArgumentException();
            var auth = getAuthObj();

            var ratingManager = RatingManager.from(auth);
            if (!ratingManager.member.beManager)
                throw new PermissionException();

            var success = ratingManager.addUserRating(request);

            if (success)
                return new RatingResponse(true);
            else
                return new RatingResponse(false).setErrorCode(EkiErrorCode.E023);

        }
        catch (ArgumentException)
        {
            return ResponseError(EkiErrorCode.E001);
        }
        catch (PermissionException)
        {
            return ResponseError(EkiErrorCode.E022);
        }
        catch (AddErrorException)
        {
            return ResponseError(EkiErrorCode.E023);
        }
        catch (Exception){ }
        return ResponseError();
    }



    public class RatingResponse : ResponseAbstractModel
    {
        public RatingResponse(bool successful) : base(successful)
        {
        }
    }
}