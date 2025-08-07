using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Authontication;
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
        [HttpPost("SendConfirmEmail")]
        public async Task<ApiResult<string>> SendConfirmEmail(string email)
        {
            var SendConfirmEmailResult = await _authonticationService.SendConfirmEmailCode(email);
            if (!SendConfirmEmailResult.IsSuccess)
            {
                return this.ToApiResult<string>(SendConfirmEmailResult);
            }
            return ApiResult<string>.Ok(SendConfirmEmailResult.Message!);
        }
        [HttpPost("SignIn")]
        public async Task<ApiResult<JwtAuthResult>> SignIn(SignInDto signInDto)
        {
            var SignInResult = await _authonticationService.SignIn(signInDto);
            if (!SignInResult.IsSuccess)
            {
                return this.ToApiResult<JwtAuthResult>(SignInResult);
            }
            return ApiResult<JwtAuthResult>.Ok(SignInResult.Value!);
        }
        [HttpPost("LogOut")]
        public async Task<ApiResult<string>> LogOut(LogOutDto logOutDto)
        {
            var LogOutResult = await _authonticationService.LogOut(logOutDto);
            if (!LogOutResult.IsSuccess)
            {
                return this.ToApiResult<string>(LogOutResult);
            }
            return ApiResult<string>.Ok(LogOutResult.Message!);
        }
        [HttpPost("RefreshAccessToken")]
        public async Task<ApiResult<JwtAuthResult>> RefreshAccessToken(RefreshAccessTokenDto refreshAccessTokenDto)
        {
            var RefreshTokenResult = await _authonticationService.RefreshAccessToken(refreshAccessTokenDto);
            if (!RefreshTokenResult.IsSuccess)
            {
                return this.ToApiResult<JwtAuthResult>(RefreshTokenResult);
            }
            return ApiResult<JwtAuthResult>.Ok(RefreshTokenResult.Value!);
        }
        [HttpPost("SendResetPasswordCode")]
        public async Task<ApiResult<string>> SendResetPasswordCode(string email)
        {
            var SendResetPasswordCodeResult = await _authonticationService.SendResetPasswordCode(email);
            if (!SendResetPasswordCodeResult.IsSuccess)
            {
                return this.ToApiResult<string>(SendResetPasswordCodeResult);
            }
            return ApiResult<string>.Ok(SendResetPasswordCodeResult.Message!);
        }
        [HttpGet("ConfirmResetPassword")]
        public async Task<ApiResult<string>> ConfirmResetPassword(string email , string code)
        {
            var ConfirmResetPasswordResult = await _authonticationService.ConfirmResetPassword(email ,code);
            if (!ConfirmResetPasswordResult.IsSuccess)
            {
                return this.ToApiResult<string>(ConfirmResetPasswordResult);
            }
            return ApiResult<string>.Ok(ConfirmResetPasswordResult.Message!);
        }
        [HttpPost("ResetPassword")]
        public async Task<ApiResult<string>> ResetPassword(string email, string password)
        {
            var ResetPasswordResult = await _authonticationService.ResetPassword(email, password);
            if (!ResetPasswordResult.IsSuccess)
            {
                return this.ToApiResult<string>(ResetPasswordResult);
            }
            return ApiResult<string>.Ok(ResetPasswordResult.Message!);
        }
    }
}
