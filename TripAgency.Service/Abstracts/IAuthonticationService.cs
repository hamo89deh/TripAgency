using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Authontication;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace TripAgency.Service.Abstracts
{
    public interface IAuthonticationService
    {
        public Task<Result<JwtAuthResult>> SignIn(SignInDto signInDto);
        public Task<Result> LogOut(LogOutDto logou);
        public Task<Result<JwtAuthResult>> RefreshAccessToken(RefreshAccessTokenDto RefreshTokenDto);
        public Task<Result> ConfirmEmail(int userId, string code);
        public Task<Result<string>> SendConfirmEmailCode(string email);
        Task<JwtAuthResult> GetJWTAndRerfreshToken(User user);
        public JwtSecurityToken ReadJWTToken(string accessToken);
        public Task<string> ValidateDetails(JwtSecurityToken jwtToken, string AccessToken, string RefreshToken);
        public Task<JwtAuthResult> GetRefreshToken(User user, JwtSecurityToken jwtToken, DateTime expiryDate, string refreshToken);
        public Task<string> ValidateToken(string AccessToken);
        public Task<Result> SendResetPasswordCode(string Email);
        public Task<Result> ConfirmResetPassword( string Email ,string Code);
        public Task<Result> ResetPassword(string Email, string Password);

    }
}
