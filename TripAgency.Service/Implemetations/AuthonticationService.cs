using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Helping;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Authontication;
using TripAgency.Service.Implementations;

namespace TripAgency.Service.Implemetations
{
    public class AuthonticationService : IAuthonticationService
    {
        private readonly TripAgencyDbContext _dBContext;
        public IUrlHelper _urlHelper { get; }
        public JwtSettings _jwtSettings { get; }
        public IRefreshTokenRepositoryAsync _refreshTokenRepository { get; }
        public UserManager<User> _userManager { get; }
        public IHttpContextAccessor _httpContextAccessor { get; }
        public IEmailService _emailService { get; }
        public AuthonticationService(UserManager<User> userManager,
                                      TripAgencyDbContext dBContext,
                                      IHttpContextAccessor httpContextAccessor,
                                      IEmailService emailService,
                                      IUrlHelper urlHelper ,
                                      JwtSettings jwtSettings,
                                      IRefreshTokenRepositoryAsync refreshTokenRepositoryAsync 
            )
        {
            _dBContext = dBContext;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _urlHelper = urlHelper;
            _jwtSettings = jwtSettings;
            _refreshTokenRepository = refreshTokenRepositoryAsync;
            _userManager = userManager;
        }
        public async Task<Result> ConfirmEmail(int userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null) 
                return Result.BadRequest($"Not Found User With Id :{userId}");

            var resultConfirmEmail = await _userManager.ConfirmEmailAsync(user, code);

            if (!resultConfirmEmail.Succeeded) 
                return Result.BadRequest("Error When Confirm Email");

            return Result.BadRequest("Success");
        }

        public async Task<Result <string>> SendConfirmEmailCode(string email)
        {
            using (var transaction = await _dBContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user is null)
                        return Result<string>.NotFound( "UserNotFound");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var resquestAccessor = _httpContextAccessor.HttpContext.Request;
                    var returnUrl = resquestAccessor.Scheme + "://" + resquestAccessor.Host + _urlHelper.Action("ConfirmEmail", "Authentications", new { userId = user.Id, code = code });
                    var message = $"To Confirm Email Click Link: <a href='{returnUrl}'>Link Of Confirmation</a>";
                    //$"/Api/V1/Authentication/ConfirmEmail?userId={user.Id}&code={code}";
                    //message or body
                    await _emailService.SendEmailAsync(user.Email, message, "ConFirm Email");
                    await transaction.CommitAsync();
                    return Result<string>.Success("Success");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return Result<string>.Failure("internal error",failureType:ResultFailureType.InternalError);
                }
            }
        }

        #region Handle Functions

        public async Task<JwtAuthResult> GetJWTToken(User user)
        {
            var (jwtToken, accessToken) = await GenerateJWTToken(user);
            var refreshToken = GetRefreshToken(user.UserName);

            var userRefreshToken = new UserRefreshToken
            {
                AddedTime = DateTime.Now,
                ExpiryDate = refreshToken.ExpireAt,
                IsUsed = true,
                IsRevoked = false,
                RefreshToken = refreshToken.TokenString,
                Token = accessToken,
                UserId = user.Id
            };
            await _refreshTokenRepository.AddAsync(userRefreshToken);

            var response = new JwtAuthResult()
            {
                refreshToken = refreshToken,
                AccessToken = accessToken
            };

            return response;
        }
        private async Task<(JwtSecurityToken, string)> GenerateJWTToken(User user)
        {
            var claims = await GetClaims(user);
          
            var jwtToken = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.Now.AddDays(_jwtSettings.AccessTokenExpireDate),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)), SecurityAlgorithms.HmacSha256Signature)
                );
            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return (jwtToken, accessToken);
        }
        private RefreshTokenResult GetRefreshToken(string username)
        {
            var refreshToken = new RefreshTokenResult
            {
                ExpireAt = DateTime.Now.AddDays(_jwtSettings.RefreshTokenExpireDate),
                UserName = username,
                TokenString = GenerateRefreshToken()
            };
            return refreshToken;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            var randomNumberGenerate = RandomNumberGenerator.Create();
            randomNumberGenerate.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public async Task<List<Claim>> GetClaims(User user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(nameof(UserClaimModel.Id), user.Id.ToString())
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);
            return claims;
        }
      
     

        #endregion

    }
    
}
