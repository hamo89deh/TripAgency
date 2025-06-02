
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using TripAgency.Bases;

namespace TripAgency.Middleware
{
    public class ErrorHandlerExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerExceptionMiddleware> _logger;

        public ErrorHandlerExceptionMiddleware(RequestDelegate next, ILogger<ErrorHandlerExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request processing.");
                await HandleExceptionAsync(httpContext, ex);
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            int statusCode = (int)HttpStatusCode.InternalServerError;
            string message = "An unexpected error occurred. Please try again later.";

            List<string> errors = new List<string>();


            if (exception is ArgumentException || exception is InvalidOperationException || exception is FormatException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
                errors.Add(exception.Message);
            }

            else if (exception is KeyNotFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;
                message = exception.Message;
                errors.Add(exception.Message);
            }
            else if (exception is ValidationException)
            {
                statusCode = (int)HttpStatusCode.UnprocessableEntity;
                message = exception.Message;
                errors.Add(exception.Message);
            }

            else if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                message = "You are not authorized to perform this action.";
            }

            else if (exception is DbUpdateException dbUpdateEx)
            {
                statusCode = (int)HttpStatusCode.Conflict; // 409 Conflict
                message = "A database conflict occurred. Please try again or check unique constraints.";
                //logger.LogError(dbUpdateEx, "DbUpdateException occurred."); // سجل تفاصيل DbUpdateException

                errors.Add("Database update failed.");
            }
   
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                message = "internal service error.";
                errors.Add(exception.InnerException is null ? exception.Message : exception.InnerException.Message);
            }
            // 4. بناء استجابة ApiResponse
            var apiResponse = ApiResponse<object>.ErrorResponse(statusCode, message, errors);
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }
}
