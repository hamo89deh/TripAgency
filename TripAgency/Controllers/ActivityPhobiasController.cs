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
    public class ActivityPhobiasController : ControllerBase
    {
        public ActivityPhobiasController(IActivityPhobiasService activityPhobiaService, IMapper mapper)
        {
            _activityPhobiaService = activityPhobiaService;
            _mapper = mapper;
        }

        public IActivityPhobiasService _activityPhobiaService { get; }
        public IMapper _mapper { get; }

        [HttpGet("{ActivityId}")]
        public async Task<ApiResult<GetActivityPhobiasDto>> GetActivityPhobiasDto(int ActivityId)
        {
            var ActivityPhobiasResult = await _activityPhobiaService.GetActivityPhobiasAsync(ActivityId);
            if (!ActivityPhobiasResult.IsSuccess)
                return this.ToApiResult(ActivityPhobiasResult);
            return ApiResult<GetActivityPhobiasDto>.Ok(ActivityPhobiasResult.Value!);
        }
        [HttpPost]
        public async Task<ApiResult<string>> AddActivityPhobias(AddActivityPhobiasDto activityPhobiasDto)
        {
            var AddActivityPhobiasResult = await _activityPhobiaService.AddActivityPhobias(activityPhobiasDto);
            if (!AddActivityPhobiasResult.IsSuccess)
            {
                return this.ToApiResult<string>(AddActivityPhobiasResult);
            }
            return ApiResult<string>.Created(AddActivityPhobiasResult.Message!);
        }

        [HttpDelete("{PhobiaId}/Activity/{ActivityId}")]
        public async Task<ApiResult<string>> DeleteActivityPhobia(int ActivityId, int PhobiaId)
        {
            var DeleteActivityPhobiaResult = await _activityPhobiaService.DeleteActivityPhobia(ActivityId, PhobiaId);
            if (!DeleteActivityPhobiaResult.IsSuccess)
                return this.ToApiResult<string>(DeleteActivityPhobiaResult);
            return ApiResult<string>.Ok("Success Delete");
        }

    }
}
