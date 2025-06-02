using Microsoft.AspNetCore.Mvc;
using TripAgency.Bases;
using TripAgency.Data.Result.TripAgency.Core.Results;

namespace TripAgency.Api.Extention
{
    public static class ControllerExtensions
    {
      
        public static ApiResult<TValue> ToApiResult<TValue>(this ControllerBase controller, Result<TValue> result)
            where TValue : class
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("This method should only be called for a failed Result.");
            }

            return result.FailureType switch
            {
                ResultFailureType.NotFound => ApiResult<TValue>.NotFound(result.Message),
                ResultFailureType.BadRequest => ApiResult<TValue>.BadRequest(result.Message, result.Errors),
                ResultFailureType.Conflict => ApiResult<TValue>.Conflict(result.Message),
                ResultFailureType.Unauthorized => ApiResult<TValue>.BadRequest(result.Message), 
                ResultFailureType.Forbidden => ApiResult<TValue>.BadRequest(result.Message), 
                _ => ApiResult<TValue>.InternalServerError(result.Message) 
            };

        }
        public static ApiResult<TValue> ToApiResult<TValue>(this ControllerBase controller, Result result)
           where TValue : class
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException("This method should only be called for a failed Result.");
            }
            return result.FailureType switch
            {
                ResultFailureType.NotFound => ApiResult<TValue>.NotFound(result.Message),
                ResultFailureType.BadRequest => ApiResult<TValue>.BadRequest(result.Message, result.Errors),
                ResultFailureType.Conflict => ApiResult<TValue>.Conflict(result.Message),
                ResultFailureType.Unauthorized => ApiResult<TValue>.BadRequest(result.Message),
                ResultFailureType.Forbidden => ApiResult<TValue>.BadRequest(result.Message),
                _ => ApiResult<TValue>.InternalServerError(result.Message)
            };
        }


    }
}