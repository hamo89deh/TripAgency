using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Commands;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageTripDestinationActivitesController : ControllerBase
    {
        public PackageTripDestinationActivitesController(IPackageTripDestinationActivityService packageTripDestinationActivityService, IMapper mapper)
        {
            _packageTripDestinationActivityService = packageTripDestinationActivityService;
            _mapper = mapper;
        }

        public IPackageTripDestinationActivityService _packageTripDestinationActivityService { get; }
        public IMapper _mapper { get; }

        [HttpPost]
        public async Task<ApiResult<GetPackageTripDestinationActivitiesDto>> AddPackageTripDestinationActivity([FromBody]AddPackageTripDestinationActivityDto packageTripDestinationActivity)
        {
            var packageTripDestinationActivityResult = await _packageTripDestinationActivityService.CreateAsync(packageTripDestinationActivity);
            if (!packageTripDestinationActivityResult.IsSuccess)
            {
                return this.ToApiResult(packageTripDestinationActivityResult);
            }
            return ApiResult<GetPackageTripDestinationActivitiesDto>.Created(packageTripDestinationActivityResult.Value!);
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdatePackageTripDestinationActivity([FromForm]UpdatePackageTripDestinationActivityDto updatePackageTripDestinationActivity)
        {
            var packageTripDestinationActivityResult = await _packageTripDestinationActivityService.UpdateAsync(updatePackageTripDestinationActivity.Id, updatePackageTripDestinationActivity);
            if (!packageTripDestinationActivityResult.IsSuccess)
                return this.ToApiResult<string>(packageTripDestinationActivityResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeletePackageTripDestinationActivity(int id)
        {
            var packageTripDestinationActivityResult = await _packageTripDestinationActivityService.DeleteAsync(id);
            if (!packageTripDestinationActivityResult.IsSuccess)
                return this.ToApiResult<string>(packageTripDestinationActivityResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }

}
