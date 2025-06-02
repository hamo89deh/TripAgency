using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TripAgency.Bases
{
    public class ApiResult<T> : IActionResult
    {
        private readonly ApiResponse<T> _apiResponse;

        public ApiResult(ApiResponse<T> apiResponse)
        {
            _apiResponse = apiResponse;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_apiResponse)
            {
                StatusCode = _apiResponse.StatusCode 
                

            };

            await objectResult.ExecuteResultAsync(context);
        }


        public static ApiResult<T> Ok(T data, string message = "Operation successful.")
        {
            return new ApiResult<T>(ApiResponse<T>.SuccessResponse((int)HttpStatusCode.OK, data, message));
        }

        public static ApiResult<T> Created(T data, string message = "Resource created successfully.")
        {
            return new ApiResult<T>(ApiResponse<T>.SuccessResponse((int)HttpStatusCode.Created, data, message));
        }

        public static ApiResult<T> NoContent(string message = "Operation successful.")
        {
            return new ApiResult<T>(ApiResponse<T>.SuccessResponse((int)HttpStatusCode.NoContent, default(T), message));
        }

        public static ApiResult<T> BadRequest(string message = "Bad Request.", List<string>? errors = null)
        {
            return new ApiResult<T>(ApiResponse<T>.BadRequestResponse(message, errors));
        }

        public static ApiResult<T> NotFound(string message = "Resource not found.")
        {
            return new ApiResult<T>(ApiResponse<T>.NotFoundResponse(message));
        }
        public static ApiResult<T> Conflict(string message = "Confilct.")
        {
            return new ApiResult<T>(ApiResponse<T>.ConflictResponse(message));
        }
        public static ApiResult<T> InternalServerError(string message = "Internal Server Error.")
        {
            return new ApiResult<T>(ApiResponse<T>.InternalServerErrorResponse(message));
        }

    }
}
