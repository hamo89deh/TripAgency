using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Data.Helping;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Activity.Commands;
using TripAgency.Service.Feature.Activity.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [Authorize(Roles = "User,Admin")]
        public async Task<ApiResult<IEnumerable<GetActivitiesDto>>> GetActivities()
        {
            var activitiesResult = await _activityService.GetAllAsync();
            if (!activitiesResult.IsSuccess)
                return this.ToApiResult(activitiesResult);
            return ApiResult<IEnumerable<GetActivitiesDto>>.Ok(activitiesResult.Value!);
        }
        [HttpPost("pagination")]
        public async Task<ApiResult<PaginatedResult<GetActivitiesDto>>> GetActivitiesaPagination(QueryParameters queryParameters)
        {
            var activitiesResult = await _activityService.GetActivityPagination(queryParameters.SearchTerm, queryParameters.Filters, queryParameters.SortColumn, queryParameters.SortDirection, queryParameters.PageNumber, queryParameters.PageSize);
            if (!activitiesResult.IsSuccess)
                return this.ToApiResult(activitiesResult);
            return ApiResult<PaginatedResult<GetActivitiesDto>>.Ok(activitiesResult.Value!);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "User,Admin")]
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
        [Authorize(Roles ="Admin")]

        public async Task<ApiResult<GetActivityByIdDto>> AddActivity(AddActivityDto activity)
        {
            var activityResult = await _activityService.CreateAsync(activity);
            if (!activityResult.IsSuccess)
            {
                return this.ToApiResult(activityResult);
            }
            return ApiResult<GetActivityByIdDto>.Created(activityResult.Value!);
        }
        [HttpPut("{Id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ApiResult<string>> UpdateActivity(int Id,UpdateActivityDto updateActivity)
        {
            var activityResult = await _activityService.UpdateAsync(Id, updateActivity);
            if (!activityResult.IsSuccess)
                return this.ToApiResult<string>(activityResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ApiResult<string>> DeleteActivity(int Id)
        {
            var activityResult = await _activityService.DeleteAsync(Id);
            if (!activityResult.IsSuccess)
                return this.ToApiResult<string>(activityResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }

}
