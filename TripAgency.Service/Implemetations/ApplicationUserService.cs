using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Context;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.User.Command;
using TripAgency.Service.Feature.User.Queries;
using TripAgency.Service.Implementations;
using Microsoft.EntityFrameworkCore;
using Azure;
using Microsoft.Extensions.Hosting;
using TripAgency.Data.Helping;

namespace TripAgency.Service.Implemetations
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly TripAgencyDbContext _dBContext;
        public IUrlHelper _urlHelper { get; }
        public IMediaService _mediaService { get; }

        public ApplicationUserService(UserManager<User> userManager ,
                                      TripAgencyDbContext dBContext,
                                      IHttpContextAccessor httpContextAccessor,
                                      IEmailService emailService,
                                      ICurrentUserService currentUserService,
                                      IUrlHelper urlHelper,
                                      IMediaService mediaService
            ) 
        {
            _dBContext = dBContext;

            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _currentUserService = currentUserService;
            _urlHelper = urlHelper;
            _mediaService = mediaService;
            _userManager = userManager;
        }

        public UserManager<User> _userManager { get; }
        public IHttpContextAccessor _httpContextAccessor { get; }
        public IEmailService _emailService { get; }
        public ICurrentUserService _currentUserService { get; }

        public async Task<Result> AddUser(AddUserDto userDtos)
        {
            var user = new User
            {
                UserName= userDtos.UserName,
                Email = userDtos.Email,
                FirstName = userDtos.FirstName ,
                LastName= userDtos.LastName,
                LoyaltyPoints = 0,
                ImageUrl = "/User/UserDefaultImage.jpg"
            };
            var trans = await _dBContext.Database.BeginTransactionAsync();
            try
            {
                //if Email is Exist
                var existUser = await _userManager.FindByEmailAsync(user.Email);
                //email is Exist
                if (existUser != null) 
                    return Result.BadRequest("Email is Exist");


                //if User is Exist
                var existUserName = await _userManager.FindByEmailAsync(user.Email);
                //email is Exist
                if (existUserName != null)
                    return Result.BadRequest("UserName is Exist");

                //Create
                var createResult = await _userManager.CreateAsync(user, userDtos.Password);
                //Failed
                if (!createResult.Succeeded)
                    return Result.BadRequest(string.Join(",", createResult.Errors.Select(x => x.Description).ToList()));

                await _userManager.AddToRoleAsync(user, "User");

                //Send Confirm Email
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var resquestAccessor = _httpContextAccessor.HttpContext!.Request;
                var returnUrl = resquestAccessor.Scheme + "://" + resquestAccessor.Host +
                   _urlHelper.Action("ConfirmEmail", "Authentications", new { userId = user.Id, code = code });
                var message = $"To Confirm Email Click Link: <a href='{returnUrl}'>Link Of Confirmation</a>";
                //$"/Api/Authentication/ConfirmEmail?userId={user.Id}&code={code}";

                //message or body
             //  await _emailService.SendEmailAsync(user.Email, message, "ConFirm Email");
            
                await trans.CommitAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                await trans.RollbackAsync();
                return Result.Failure("Internal error" , failureType: ResultFailureType.InternalError);
            }
        }

        public async Task<Result<string>> ChangePassword(ChangeUserPassword changeUserPassword)
        {
            //get user
            //check if user is exist
            var user = await _userManager.FindByIdAsync(changeUserPassword.UserId.ToString());
            //if Not Exist notfound
            if (user == null)
                return Result<string>.NotFound($"Not Found User With Id : {changeUserPassword.UserId}");

            //Change User Password
            var result = await _userManager.ChangePasswordAsync(user, changeUserPassword.CurrentPassword, changeUserPassword.NewPassword);

            //result
            if (!result.Succeeded) 
                return Result<string>.BadRequest(string.Join(",", result.Errors.Select(e => e.Description)));
            return Result<string>.Success("Change Password Success");

        }
        //TODO
        public async Task<Result> DeleteUser(int Id)
        {          
            //check if user is exist
            var User = await _userManager.FindByIdAsync(Id.ToString());
            //if Not Exist notfound
            if (User == null) return Result.NotFound($"Not Found User By Id : {Id}");

            //update
            var result = await _userManager.DeleteAsync(User);
            //result is not success
            if (!result.Succeeded)
                return Result.BadRequest("Error When Delete User");
            //message
            return Result.Success();
        }

        public async Task<Result> UpdateUser(UpdateUserDto userDtos)
        {
            //check if user is exist
            var oldUser = await _currentUserService.GetUserAsync();

            oldUser.FirstName = userDtos.FirstName;
            oldUser.LastName = userDtos.LastName;
            oldUser.Email = userDtos.Email;
            oldUser.PhoneNumber = userDtos.PhoneNumber;
            oldUser.Address =   userDtos.Address;
            oldUser.Country = userDtos.Country;
            if(userDtos.Image is not null)
                oldUser.ImageUrl = await _mediaService.UploadMediaAsync("User",userDtos.Image);

            //if username is Exist
            var userByUserName = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userDtos.UserName && x.Id != oldUser.Id);
           // username is Exist
            if (userByUserName != null)
                return Result.BadRequest("User Name Is Already Token ");
            //if Eamil is Exist
            var userByEmail = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == userDtos.Email && x.Id != oldUser.Id);
            // username is Exist
            if (userByEmail != null)
                return Result.BadRequest("Email Is Already Token ");
            //update
            var result = await _userManager.UpdateAsync(oldUser);
            //result is not success
            if (!result.Succeeded)
                return Result.BadRequest(string.Join(",", result.Errors.Select(x => x.Description).ToList()));
            //message
            return Result.Success();

        }

        public async Task<Result<GetUserByIdDto>> GetUserById(int Id)
        {
            var user = await _userManager.FindByIdAsync(Id.ToString());
            if (user == null) 
                return Result<GetUserByIdDto>.NotFound($"Not Found User With Id : {Id}");
      
            var result = new GetUserByIdDto
            {
                Id = Id,
                Address = user.Address,
                Country = user.Country,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ImageUrl = user.ImageUrl ,
                UserName = user.UserName,
                LoyaltyPoints = user.LoyaltyPoints
            };
            
            return Result<GetUserByIdDto>.Success(result);
        }

        public async Task<Result<IEnumerable<GetUsersDto>>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            if (users.Count() == 0)
                return Result<IEnumerable<GetUsersDto>>.NotFound($"Not Found Users");

            var usersResult = new List<GetUsersDto>();
            foreach ( var user in users )
            {
                usersResult.Add(new GetUsersDto
                {
                    Id = user.Id,
                    Address = user.Address,
                    Country = user.Country,
                    Email = user.Email!,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ImageUrl= user.ImageUrl,
                    UserName = user.UserName,
                    LoyaltyPoints = user.LoyaltyPoints
                });
            }

            return Result<IEnumerable<GetUsersDto>>.Success(usersResult);
        }

        public async Task<Result<PaginatedResult<GetUsersDto>>> GetUsersPagination(string? search, int pageNumber, int pageSize )
        {
            var query = _userManager.Users;
            if(search is not null)
                query = query.ApplySearch(search,new[]{ "UserName","Email"});
         
            var usersResult = await query.Select(user => new GetUsersDto()
            {
                Id = user.Id,
                Address = user.Address,
                Country = user.Country,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ImageUrl = user.ImageUrl,
                UserName = user.UserName,
                LoyaltyPoints = user.LoyaltyPoints
                
            }).ToPaginatedListAsync(pageNumber, pageSize);
            if (usersResult.TotalCount == 0)
                 return Result<PaginatedResult<GetUsersDto>>.NotFound($"Not Found Users");
            return Result<PaginatedResult<GetUsersDto>>.Success(usersResult);
        }
    }

}
