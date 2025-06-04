using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Activity.Commands;
using TripAgency.Service.Feature.Activity.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        public ActivitiesController(IActivityService activityService, IMapper mapper)
        {
            _activityService = activityService;
            _mapper = mapper;
        }

        public IActivityService _activityService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetActivitiesDto>>> GetActivities()
        {
            var activitiesResult = await _activityService.GetAllAsync();
            if (!activitiesResult.IsSuccess)
                return this.ToApiResult(activitiesResult);
            return ApiResult<IEnumerable<GetActivitiesDto>>.Ok(activitiesResult.Value!);
        }
        [HttpGet("{id}")]
        public async Task<ApiResult<GetActivityByIdDto>> GetActivityById(int id)
        {
            var activityResult = await _activityService.GetByIdAsync(id);
            if (!activityResult.IsSuccess)
                return this.ToApiResult(activityResult);
            return ApiResult<GetActivityByIdDto>.Ok(activityResult.Value!);
        }

        [HttpGet("Name/{name}")]
        public async Task<ApiResult<GetActivityByIdDto>> GetActivityByName(string name)
        {
            var activityResult = await _activityService.GetActivityByNameAsync(name);
            if (!activityResult.IsSuccess)
                return this.ToApiResult(activityResult);
            return ApiResult<GetActivityByIdDto>.Ok(activityResult.Value!);
        }
        [HttpPost]
        public async Task<ApiResult<GetActivityByIdDto>> AddActivity(AddActivityDto activity)
        {
            var activityResult = await _activityService.CreateAsync(activity);
            if (!activityResult.IsSuccess)
            {
                return this.ToApiResult(activityResult);
            }
            return ApiResult<GetActivityByIdDto>.Created(activityResult.Value!);
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdateActivity(UpdateActivityDto updateActivity)
        {
            var activityResult = await _activityService.UpdateAsync(updateActivity.Id, updateActivity);
            if (!activityResult.IsSuccess)
                return this.ToApiResult<string>(activityResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteActivity(int id)
        {
            var activityResult = await _activityService.DeleteAsync(id);
            if (!activityResult.IsSuccess)
                return this.ToApiResult<string>(activityResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }

}
