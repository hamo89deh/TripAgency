using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Data.Helping;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.User.Command;
using TripAgency.Service.Feature.User.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUsersController : ControllerBase
    {
        public IApplicationUserService _applicationUserService { get; }
        public ICurrentUserService CurrentUserService { get; }

        public ApplicationUsersController(IApplicationUserService applicationUser , ICurrentUserService currentUserService)
        {
            _applicationUserService = applicationUser;
            CurrentUserService = currentUserService;
        }

        [HttpPost]
        public async Task<ApiResult<string>> AddNewUser(AddUserDto userDto)
        {
            var AddNewUserResult = await _applicationUserService.AddUser(userDto);
            if (!AddNewUserResult.IsSuccess)
            {
                return this.ToApiResult<string>(AddNewUserResult);
            }
            return ApiResult<string>.Created(AddNewUserResult.Message!);
        }
        [HttpPut()]
        [Authorize(Roles = "User,Admin")]
        public async Task<ApiResult<string>> UpdateUser([FromForm]UpdateUserDto userDto)
        {
            var UpdateUserResult = await _applicationUserService.UpdateUser(userDto);
            if (!UpdateUserResult.IsSuccess)
            {
                return this.ToApiResult<string>(UpdateUserResult);
            }
            return ApiResult<string>.Ok(UpdateUserResult.Message!);

        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{Id}")]
        public async Task<ApiResult<string>> DeleteUser(int Id)
        {
            var DeleteUserResult = await _applicationUserService.DeleteUser(Id);
            if (!DeleteUserResult.IsSuccess)
            {
                return this.ToApiResult<string>(DeleteUserResult);
            }
            return ApiResult<string>.Ok(DeleteUserResult.Message!);

        }
        [HttpGet("UserInformation")]
        [Authorize(Roles = "Admin,User")]
        public async Task<ApiResult<GetUserByIdDto>> GetUserInformation()
        {
            var user = await CurrentUserService.GetUserAsync();
            var UserResult = await _applicationUserService.GetUserById(user.Id);
            if (!UserResult.IsSuccess)
            {
                return this.ToApiResult<GetUserByIdDto>(UserResult);
            }
            return ApiResult<GetUserByIdDto>.Ok(UserResult.Value!);

        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ApiResult<IEnumerable<GetUsersDto>>> GetUsers()
        {
            var UsersResult = await _applicationUserService.GetUsers();
            if (!UsersResult.IsSuccess)
            {
                return this.ToApiResult<IEnumerable<GetUsersDto>>(UsersResult);
            }
            return ApiResult<IEnumerable<GetUsersDto>>.Ok(UsersResult.Value!);

        }
        [HttpGet("Pagination")]
        [Authorize(Roles = "Admin")]
        public async Task<ApiResult<PaginatedResult<GetUsersDto>>> GetUsers(string? search , int pageNumber=1 , int pageSize =10)
        {
            var UsersResult = await _applicationUserService.GetUsersPagination(search,pageNumber , pageSize);
            if (!UsersResult.IsSuccess)
            {
                return this.ToApiResult<PaginatedResult<GetUsersDto>>(UsersResult);
            }
            return ApiResult<PaginatedResult<GetUsersDto>>.Ok(UsersResult.Value!);

        }
    }
}
