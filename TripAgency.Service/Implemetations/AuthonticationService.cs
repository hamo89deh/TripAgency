using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
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
using TripAgency.Infrastructure.Migrations;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Authontication;
using TripAgency.Service.Implementations;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace TripAgency.Service.Implemetations
{
    public class AuthonticationService : IAuthonticationService
    {
        private readonly TripAgencyDbContext _dBContext;
        public IUrlHelper _urlHelper { get; }
        public JwtSettings _jwtSettings { get; }
        public IRefreshTokenRepositoryAsync _refreshTokenRepository { get; }
        private UserManager<User> _userManager { get; }
        private SignInManager<User> _signInManager;
        public IHttpContextAccessor _httpContextAccessor { get; }
        public IEmailService _emailService { get; }
        public AuthonticationService(UserManager<User> userManager,
                                      TripAgencyDbContext dBContext,
                                      IHttpContextAccessor httpContextAccessor,
                                      IEmailService emailService,
                                      IUrlHelper urlHelper ,
                                      JwtSettings jwtSettings,
                                      IRefreshTokenRepositoryAsync refreshTokenRepositoryAsync ,
                                      SignInManager<User> signInManager
            )
        {
            _dBContext = dBContext;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _urlHelper = urlHelper;
            _jwtSettings = jwtSettings;
            _refreshTokenRepository = refreshTokenRepositoryAsync;
            _userManager = userManager;
            _signInManager = signInManager;
        }


        #region Handle Functions
        public async Task<Result<string>> SendConfirmEmailCode(string email)
        {
            using (var transaction = await _dBContext.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user is null)
                        return Result<string>.NotFound($"Not Found User With Email : {email}");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var resquestAccessor = _httpContextAccessor.HttpContext!.Request;
                    var returnUrl = resquestAccessor.Scheme + "://" + resquestAccessor.Host + _urlHelper.Action("ConfirmEmail", "Authentications", new { userId = user.Id, code = code });
                    var message = $"To Confirm Email Click Link: <a href='{returnUrl}'>Link Of Confirmation</a>";
                    //$"/Api/V1/Authentication/ConfirmEmail?userId={user.Id}&code={code}";
                    //message or body
                    await _emailService.SendEmailAsync(user.Email!, message, "Confirm Email");
                    await transaction.CommitAsync();
                    return Result<string>.Success("Success");
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return Result<string>.Failure("internal error", failureType: ResultFailureType.InternalError);
                }
            }
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
        public async Task<Result<JwtAuthResult>> RefreshAccessToken(RefreshAccessTokenDto RefreshAccessTokenDto)
        {
            JwtSecurityToken jwtToken ;
            ClaimsPrincipal principal;
            try
            {
                principal = GetPrincipalFromExpiredToken(RefreshAccessTokenDto.AccessToken);
                var handler = new JwtSecurityTokenHandler();
                jwtToken = handler.ReadJwtToken(RefreshAccessTokenDto.AccessToken);
            }
            catch (Exception)
            {
                return Result<JwtAuthResult>.BadRequest("Invalid AccessToken");
                throw;
            }
       
            var ErrorDetail = await ValidateDetails(jwtToken, RefreshAccessTokenDto.AccessToken, RefreshAccessTokenDto.RefreshToken);
            
            switch (ErrorDetail)
            {
                case "AlgorithmIsWrong" : return Result<JwtAuthResult>.Failure("Algorithm Is Wrong" , failureType: ResultFailureType.Unauthorized);
                case "TokenIsNotExpired": return Result<JwtAuthResult>.Failure("Token Is Not Expired", failureType: ResultFailureType.Unauthorized);
                case "RefreshTokenIsNotFound": return Result<JwtAuthResult>.Failure("RefreshToken Is NotFound", failureType: ResultFailureType.Unauthorized);
                case "RefreshTokenIsExpired": return Result<JwtAuthResult>.Failure("RefreshToken Is Expired", failureType: ResultFailureType.Unauthorized);
            }
            var userId = principal.FindFirstValue(nameof(UserClaimModel.Id));
            var user = await _userManager.FindByIdAsync(userId!);
            if (user == null)
            {
                return Result<JwtAuthResult>.NotFound("Not Found User For This AccessToken and RefreshToken");
            }
            var oldRefreshToken = await _refreshTokenRepository.GetTableNoTracking()
                                                               .FirstOrDefaultAsync(r => r.RefreshToken == RefreshAccessTokenDto.RefreshToken);
            // حذف  Refresh Token في قاعدة البيانات
            await _refreshTokenRepository.DeleteAsync(oldRefreshToken!);

            var result = await GetJWTAndRerfreshToken(user);
            return Result<JwtAuthResult>.Success(result);
        }
        public async Task<Result<JwtAuthResult>> SignIn(SignInDto signInDto)
        {
            var user = await _userManager.FindByEmailAsync(signInDto.Email);

            if (user is null)
                return Result<JwtAuthResult>.Failure($"Not Found User By Email Or Password Not Correct : {signInDto.Email} ", failureType: ResultFailureType.Unauthorized);

            if (!user.EmailConfirmed)
                return Result<JwtAuthResult>.BadRequest("Email Not Confirm");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(user, signInDto.Password, false);

            if (!signInResult.Succeeded)
                return Result<JwtAuthResult>.Failure("Not Found User By Email Or Password Not Correct", failureType: ResultFailureType.Unauthorized);

            var oldRefreshToken = await _refreshTokenRepository.GetTableNoTracking()
                                                               .Where(r => r.UserId == user.Id)
                                                               .FirstOrDefaultAsync();
            if (oldRefreshToken is not null)
                await _refreshTokenRepository.DeleteAsync(oldRefreshToken);

            var result = await GetJWTAndRerfreshToken(user);

            return Result<JwtAuthResult>.Success(result);
        }       
       
        public async Task<Result> LogOut(LogOutDto logOutDto)
        {
            var token = await _refreshTokenRepository.GetTableNoTracking()
                                                     .FirstOrDefaultAsync(t => t.Token == logOutDto.RefreshToken);

            if (token != null)
            {
                token.IsRevoked = true;
                await _refreshTokenRepository.UpdateAsync(token);
            }
            return Result.Success("Log out Seuccess");
        }

        public async Task<Result> SendResetPasswordCode(string Email)
        {
            var trans = await _dBContext.Database.BeginTransactionAsync();
            try
            {
                //user
                var user = await _userManager.FindByEmailAsync(Email);

                //user not Exist => not found
                if (user == null)
                    return Result.NotFound($"Not Found User With Email : {Email}");

                //Generate Random Number
                var chars = "0123456789";
                var random = new Random();
                var randomNumber = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());

                //update User In Database Code
                user.Code = randomNumber;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    return Result.Failure("Error When Update User", failureType: ResultFailureType.InternalError);

                var message = "Code To Reset Passsword : " + user.Code;

                //Send Code To  Email 
                await _emailService.SendEmailAsync(user.Email, message, "Reset Password");
                await trans.CommitAsync();
                return Result.Success("Send Reset Password Code Seccusss ");
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                return Result.Failure("Internal Error", failureType: ResultFailureType.InternalError);
            }
        }
        public async Task<Result> ConfirmResetPassword(string Email ,string Code)
        {
            //Get User
            //user
            var user = await _userManager.FindByEmailAsync(Email);
            //user not Exist => not found
            if (user == null)
                return Result.NotFound($"Not Found User With Email : {Email}");
            //Decrept Code From Database User Code
            var userCode = user.Code;
            //Equal With Code
            if (userCode != Code)
                return Result.BadRequest("The Code Not Correct");

            return Result.Success("confirm code success");
        }
        public async Task<Result> ResetPassword(string Email, string Password)
        {
            var trans = await _dBContext.Database.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByEmailAsync(Email);

                if (user == null)
                    return Result.NotFound($"Not Found User With Email : {Email}");
                var RemovePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!RemovePasswordResult.Succeeded)
                {
                    return Result.Failure("Error when Delete old password", failureType: ResultFailureType.InternalError);
                }
                if (!await _userManager.HasPasswordAsync(user))
                {
                    await _userManager.AddPasswordAsync(user, Password);
                }
                await trans.CommitAsync();
                return Result.Success("Rest Password Success");
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                return Result.Failure("internal error", failureType: ResultFailureType.InternalError);
            }
        }

     
        public async Task<JwtAuthResult> GetJWTAndRerfreshToken(User user)
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
                RefreshToken = refreshToken,
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
      
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = false // تجاهل انتهاء الصلاحية
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;
        }   
       
        public async Task<JwtAuthResult> GetRefreshToken(User user, JwtSecurityToken jwtToken, DateTime expiryDate, string refreshToken)
        {
            var (jwtSecurityToken, newToken) = await GenerateJWTToken(user);
            var response = new JwtAuthResult();
            response.AccessToken = newToken;
            var refreshTokenResult = new RefreshTokenResult();
            refreshTokenResult.UserName = user.UserName!;
            refreshTokenResult.TokenString = refreshToken;
            refreshTokenResult.ExpireAt = expiryDate;
            response.RefreshToken = refreshTokenResult;
            return response;

        }
        public JwtSecurityToken ReadJWTToken(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }
            var handler = new JwtSecurityTokenHandler();
            var response = handler.ReadJwtToken(accessToken);
            return response;
        }
        public async Task<string> ValidateToken(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidIssuers = new[] { _jwtSettings.Issuer },
                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.Secret)),
                ValidAudience = _jwtSettings.Audience,
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidateLifetime = false,
            };
            try
            {
                var validator = handler.ValidateToken(accessToken, parameters, out SecurityToken validatedToken);

                if (validator == null)
                {
                    return "InvalidToken";
                }

                return "NotExpired";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public async Task<string> ValidateDetails(JwtSecurityToken jwtToken, string accessToken, string refreshToken)
        {
            if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature))
            {
                return "AlgorithmIsWrong";
            }
            //if (jwtToken.ValidTo > DateTime.UtcNow)
            //{
            //    return "TokenIsNotExpired";
            //}

            //Get User

            var userId = jwtToken.Claims.FirstOrDefault(x => x.Type == nameof(UserClaimModel.Id))!.Value;
            
            var userRefreshToken = await _refreshTokenRepository.GetTableNoTracking()
                                             .FirstOrDefaultAsync(x => x.Token == accessToken &&
                                                                       x.RefreshToken == refreshToken &&
                                                                       x.UserId == int.Parse(userId));
            if (userRefreshToken == null)
            {
                return "RefreshTokenIsNotFound";
            }

            if (userRefreshToken.ExpiryDate < DateTime.UtcNow)
            {
                userRefreshToken.IsRevoked = true;
                userRefreshToken.IsUsed = false;
                await _refreshTokenRepository.UpdateAsync(userRefreshToken);
                return "RefreshTokenIsExpired";
            }
            return string.Empty;
        }
      
     
        #endregion
    }
 }