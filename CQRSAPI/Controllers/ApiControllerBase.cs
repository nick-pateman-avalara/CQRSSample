using System;
using CQRSAPI.Extensions;
using CQRSAPI.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace CQRSAPI.Controllers
{

    public class ApiControllerBase : Controller
    {

        public virtual ILogger Logger { get; protected set; }

        protected IActionResult ProcessApiResponse<T>(
            ApiResponse<T> apiResponse,
            object notFoundReturnValue = null,
            object conflictReturnValue = null)
        {
            switch (apiResponse.Result)
            {
                case ApiResponse<T>.ResponseType.Ok:
                    {
                        return (Ok(apiResponse.Value));
                    }
                case ApiResponse<T>.ResponseType.BadRequest:
                    {
                        Logger?.LogError("Bad Request.");
                        return (BadRequest(apiResponse.Errors.ToStringList()));
                    }
                case ApiResponse<T>.ResponseType.NotFound:
                    {
                        Logger?.LogError("Not Found.");
                        return (NotFound(notFoundReturnValue));
                    }
                case ApiResponse<T>.ResponseType.Conflict:
                    {
                        Logger?.LogWarning("Conflict.");
                        return (Conflict(conflictReturnValue));
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }

        protected IActionResult InvalidModel(ModelStateDictionary modelState)
        {
            return (ProcessApiResponse(new ApiResponse<object>()
            {
                Result = ApiResponse<object>.ResponseType.BadRequest,
                Errors = modelState.ToErrorList()
            }));
        }

    }

}
