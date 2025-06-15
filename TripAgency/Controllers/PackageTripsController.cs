using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTrip.Commands;
using TripAgency.Service.Feature.PackageTrip.Queries;
using TripAgency.Service.Feature.PackageTripDestination.Commands;
using TripAgency.Service.Feature.PackageTripDestination.Queries;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Commands;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Implementations;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageTripsController : ControllerBase
    {
        public PackageTripsController(IPackageTripService packageTripService,
                                      IPackageTripDestinationService packageTripDestinationService,
                                      IPackageTripDestinationActivityService packageTripDestinationActivityService,
                                      IMapper mapper)
        {
            _packageTripService = packageTripService;
            _packageTripDestinationService = packageTripDestinationService;
            _packageTripDestinationActivityService = packageTripDestinationActivityService;
            _mapper = mapper;
        }

        public IPackageTripService _packageTripService { get; }
        public IPackageTripDestinationService _packageTripDestinationService { get; }
        public IPackageTripDestinationActivityService _packageTripDestinationActivityService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetPackageTripsDto>>> GetPackageTrips()
        {
            var packageTripsResult = await _packageTripService.GetAllAsync();
            if (!packageTripsResult.IsSuccess)
                return this.ToApiResult(packageTripsResult);
            return ApiResult<IEnumerable<GetPackageTripsDto>>.Ok(packageTripsResult.Value!);
        }
       
        [HttpGet("{id}")]
        public async Task<ApiResult<GetPackageTripByIdDto>> GetPackageTripById(int id)
        {
            var packageTripResult = await _packageTripService.GetByIdAsync(id);
            if (!packageTripResult.IsSuccess)
                return this.ToApiResult(packageTripResult);
            return ApiResult<GetPackageTripByIdDto>.Ok(packageTripResult.Value!);
        }

        [HttpPost]
        public async Task<ApiResult<GetPackageTripByIdDto>> AddPackageTrip(AddPackageTripDto packageTripdto)
        {
            var packageTripResult = await _packageTripService.CreateAsync(packageTripdto);
            if (!packageTripResult.IsSuccess)
            {
                return this.ToApiResult(packageTripResult);
            }
            return ApiResult<GetPackageTripByIdDto>.Created(packageTripResult.Value!);
        }


      
        [HttpPut]
        public async Task<ApiResult<string>> UpdatePackageTrip(UpdatePackageTripDto updatePackageTrip)
        {
            var packageTripResult = await _packageTripService.UpdateAsync(updatePackageTrip.Id, updatePackageTrip);
            if (!packageTripResult.IsSuccess)
                return this.ToApiResult<string>(packageTripResult);
            return ApiResult<string>.Ok("Success Updated");

        }
      
        [HttpDelete]
        public async Task<ApiResult<string>> DeletePackageTrip(int id)
        {
            var packageTripResult = await _packageTripService.DeleteAsync(id);
            if (!packageTripResult.IsSuccess)
                return this.ToApiResult<string>(packageTripResult);
            return ApiResult<string>.Ok("Success Delete");
        }

        [HttpPost("Destination")]
        public async Task<ApiResult<GetPackageTripDestinationByIdDto>> AddPackageTripDestination(AddPackageTripDestinationDto packageTripDestination)
        {
            var packageTripDestinationResult = await _packageTripDestinationService.CreateAsync(packageTripDestination);
            if (!packageTripDestinationResult.IsSuccess)
            {
                return this.ToApiResult(packageTripDestinationResult);
            }
            return ApiResult<GetPackageTripDestinationByIdDto>.Created(packageTripDestinationResult.Value!);
        }
        [HttpPut("Destination")]
        public async Task<ApiResult<string>> UpdatePackageTripDestination(UpdatePackageTripDestinationDto updatePackageTripDestination)
        {
            var packageTripDestinationResult = await _packageTripDestinationService.UpdateAsync(0, updatePackageTripDestination);
            if (!packageTripDestinationResult.IsSuccess)
                return this.ToApiResult<string>(packageTripDestinationResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete("{packageTripId}/Destination/{destinationId}")]
        public async Task<ApiResult<string>> DeletePackageTripDestination(int packageTripId, int destinationId)
        {
            var packageTripDstinationDtoResult = await _packageTripDestinationService.GetPackageTripDestination(packageTripId, destinationId);
            if (!packageTripDstinationDtoResult.IsSuccess)
            {
                return this.ToApiResult<string>(Result<string>.Failure(packageTripDstinationDtoResult.Message, packageTripDstinationDtoResult.Errors, packageTripDstinationDtoResult.FailureType));
            }
            var packageTripDestinationResult = await _packageTripDestinationService.DeleteAsync(packageTripDstinationDtoResult.Value!.Id);
            if (!packageTripDestinationResult.IsSuccess)
                return this.ToApiResult<string>(packageTripDestinationResult);
            return ApiResult<string>.Ok("Success Delete");
        }


        [HttpPost("Destination/Activities")]
        public async Task<ApiResult<GetPackageTripDestinationActivitiesDto>> AddPackageTripDestinationActivity( AddPackageTripDestinationActivityDto packageTripDestinationActivity)
        {
            var packageTripDestinationActivityResult = await _packageTripDestinationActivityService.CreateAsync(packageTripDestinationActivity);
            if (!packageTripDestinationActivityResult.IsSuccess)
            {
                return this.ToApiResult(packageTripDestinationActivityResult);
            }
            return ApiResult<GetPackageTripDestinationActivitiesDto>.Created(packageTripDestinationActivityResult.Value!);
        }
        
        [HttpPut("Destination/Activity")]
        public async Task<ApiResult<string>> UpdatePackageTripDestinationActivity(UpdatePackageTripDestinationActivityDto updatePackageTripDestinationActivity)
        {
            var packageTripDestinationActivityResult = await _packageTripDestinationActivityService.UpdateAsync(0, updatePackageTripDestinationActivity);
            if (!packageTripDestinationActivityResult.IsSuccess)
                return this.ToApiResult<string>(packageTripDestinationActivityResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        
        [HttpDelete("{packageTripId}/Destination/{destinationId}/Activity/{activityId}")]
        public async Task<ApiResult<string>> DeletePackageTripDestinationActivity(int packageTripId, int destinationId , int activityId)
        {
            var packageTripDestinationActivityDtoResult = await _packageTripDestinationActivityService.GetPackageTripDestinationActivity(packageTripId, destinationId, activityId);
            if (!packageTripDestinationActivityDtoResult.IsSuccess)
            {
                return this.ToApiResult<string>(Result<string>.Failure(packageTripDestinationActivityDtoResult.Message, packageTripDestinationActivityDtoResult.Errors, packageTripDestinationActivityDtoResult.FailureType));

            }
            var packageTripDestinationActivityResult = await _packageTripDestinationActivityService.DeleteAsync(packageTripDestinationActivityDtoResult.Value!.Id);
            if (!packageTripDestinationActivityResult.IsSuccess)
                return this.ToApiResult<string>(packageTripDestinationActivityResult);
            return ApiResult<string>.Ok("Success Delete");
        }

        [HttpGet("{Id}/Destinations")]
        public async Task<ApiResult<GetPackageTripDestinationsDto>> GetPackageTripDestinations(int Id)
        {
            var packageTripsResult = await _packageTripDestinationService.GetPackageTripDestinations(Id);
            if (!packageTripsResult.IsSuccess)
                return this.ToApiResult<GetPackageTripDestinationsDto>(packageTripsResult);
            return ApiResult<GetPackageTripDestinationsDto>.Ok(packageTripsResult.Value!);
        }

        [HttpGet("{PackageTripId}/Destination/{DestinationId}")]
        public async Task<ApiResult<GetPackageTripDestinationActivitiesDto>> GetPackageTripDestinationActivities(int PackageTripId, int DestinationId)
        {
            var packageTripsResult = await _packageTripDestinationService.GetPackageTripDestinationActivities(PackageTripId, DestinationId);
            if (!packageTripsResult.IsSuccess)
                return this.ToApiResult<GetPackageTripDestinationActivitiesDto>(packageTripsResult);
            return ApiResult<GetPackageTripDestinationActivitiesDto>.Ok(packageTripsResult.Value!);
        }
        [HttpGet("{PackageTripId}/Destinations/Activities")]
        public async Task<ApiResult<GetPackageTripDestinationsActivitiesDto>> GetPackageTripDestinationsActivities(int PackageTripId)
        {
            var packageTripsResult = await _packageTripDestinationService.GetPackageTripDestinationsActivities(PackageTripId);
            if (!packageTripsResult.IsSuccess)
                return this.ToApiResult<GetPackageTripDestinationsActivitiesDto>(packageTripsResult);
            return ApiResult<GetPackageTripDestinationsActivitiesDto>.Ok(packageTripsResult.Value!);
        }
    }

}
