using System;
using CQRSAPI.Extensions;
using CQRSAPI.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CQRSAPI.Controllers
{

    public class ApiControllerBase : Controller
    {

        protected IActionResult ProcessApiResponse<T>(ApiResponse<T> apiResponse)
        {
            switch (apiResponse.Result)
            {
                case ApiResponse<T>.ResponseType.Ok:
                {
                    return (Ok(apiResponse.Value));
                }
                case ApiResponse<T>.ResponseType.BadRequest:
                {
                    return (BadRequest(apiResponse.Errors.ToStringList()));
                }
                case ApiResponse<T>.ResponseType.NotFound:
                {
                    return (NotFound());
                }
                case ApiResponse<T>.ResponseType.Conflict:
                {
                    return (Conflict());
                }
                default:
                {
                    throw new NotImplementedException();
                }
            }
        }


    }

}
