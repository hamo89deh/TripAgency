using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Context;
using TripAgency.Service.Abstracts;

namespace TripAgency.Service.Implemetations
{
    public class AuthonticationService : IAuthonticationService
    {
        private readonly TripAgencyDbContext _dBContext;
        public IUrlHelper _urlHelper { get; }
        public UserManager<User> _userManager { get; }
        public IHttpContextAccessor _httpContextAccessor { get; }
        public IEmailService _emailService { get; }
        public AuthonticationService(UserManager<User> userManager,
                                      TripAgencyDbContext dBContext,
                                      IHttpContextAccessor httpContextAccessor,
                                      IEmailService emailService,
                                      IUrlHelper urlHelper)
        {
            _dBContext = dBContext;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _urlHelper = urlHelper;
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
    }
}
