using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Service.Abstracts;

namespace TripAgency.Api.Behavior
{
    public class AuthFilter : IAsyncActionFilter
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthFilter(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.User.Identity!.IsAuthenticated == true)
            {
                var roles = await _currentUserService.GetCurrentUserRolesAsync();
                if (roles.All(x => x.ToLower() != "user"))
                {
                    context.Result = new ObjectResult("Forbidden")
                    {
                        StatusCode = StatusCodes.Status403Forbidden
                    };
                }
                else
                {
                    await next();
                }


            }

        }
    }
}
