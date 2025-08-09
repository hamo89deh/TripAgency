using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecuritysController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public string Get()
        {
            return "Ok";
        }
    }
}
