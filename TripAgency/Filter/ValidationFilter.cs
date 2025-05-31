using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Bases;

namespace TripAgency.Api.Behavior
{

    public class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           
            if (!context.ModelState.IsValid)
            {
               
                var errors = context.ModelState.Values
                                    .SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();

                context.Result = new BadRequestObjectResult(
                    ApiResponse<object>.BadRequestResponse("Validation failed. Please check your input.", errors)
                );
                return;
            }

            await next();
        }
    }

}
