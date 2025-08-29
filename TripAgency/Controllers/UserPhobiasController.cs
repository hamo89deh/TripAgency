using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.ActivityPhobia.Commands;
using TripAgency.Service.Feature.ActivityPhobia.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserPhobiasController : ControllerBase
    {
        public UserPhobiasController(IUserPhobiaService userPhobiaService, IMapper mapper)
        {
            _userPhobiaService = userPhobiaService;
            _mapper = mapper;
        }

        public IUserPhobiaService _userPhobiaService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<GetUserPhobiasDto>> GetUserPhobiasDto()
        {
            var UserPhobiasResult = await _userPhobiaService.GetUserPhobiasAsync();
            if (!UserPhobiasResult.IsSuccess)
                return this.ToApiResult(UserPhobiasResult);
            return ApiResult<GetUserPhobiasDto>.Ok(UserPhobiasResult.Value!);
        }
        [HttpPost]
        public async Task<ApiResult<string>> AddUserPhobias(AddUserPhobiasDto userPhobiasDto)
        {
            var AddUserPhobiasResult = await _userPhobiaService.AddUserPhobias(userPhobiasDto);
            if (!AddUserPhobiasResult.IsSuccess)
            {
                return this.ToApiResult<string>(AddUserPhobiasResult);
            }
            return ApiResult<string>.Created(AddUserPhobiasResult.Message!);
        }

        [HttpDelete("{PhobiaId}")]
        public async Task<ApiResult<string>> DeleteUserPhobia(int PhobiaId)
        {
            var DeleteUserPhobiaResult = await _userPhobiaService.DeleteUserPhobia(PhobiaId);
            if (!DeleteUserPhobiaResult.IsSuccess)
                return this.ToApiResult<string>(DeleteUserPhobiaResult);
            return ApiResult<string>.Ok("Success Delete");
        }

    }
}
