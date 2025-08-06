using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.BookingTrip.Commands;
using TripAgency.Service.Feature.Payment;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthonticationsController : ControllerBase
    {
        public AuthonticationsController(IAuthonticationService authonticationService)
        {
            _authonticationService = authonticationService;
        }

        public IAuthonticationService _authonticationService { get; }

        [HttpGet("ConfirmEmail")]
        public async Task<ApiResult<string>> ConfirmEmail(int UserId , string Code)
        {
            var ConfirmEmailResult = await _authonticationService.ConfirmEmail(UserId, Code);
            if (!ConfirmEmailResult.IsSuccess)
            {
                return this.ToApiResult<string>(ConfirmEmailResult);
            }
            return ApiResult<string>.Ok(ConfirmEmailResult.Message!);
        }
    }
}
