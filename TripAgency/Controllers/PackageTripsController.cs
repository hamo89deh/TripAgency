using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Data.Enums;
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

        [HttpGet("Trip/{TripId}")]
        public async Task<ApiResult<GetPackageTripsForTripDto>> GetPackageTripsForTrip(int TripId)
        {
            var packageTripsResult = await _packageTripService.GetPackageTripsForTrip(TripId);
            if (!packageTripsResult.IsSuccess)
                return this.ToApiResult<GetPackageTripsForTripDto>(packageTripsResult);
            return ApiResult<GetPackageTripsForTripDto>.Ok(packageTripsResult.Value!);
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
        public async Task<ApiResult<GetPackageTripByIdDto>> AddPackageTrip([FromForm] AddPackageTripDto packageTripdto)
        {
            var packageTripResult = await _packageTripService.CreateAsync(packageTripdto);
            if (!packageTripResult.IsSuccess)
            {
                return this.ToApiResult(packageTripResult);
            }
            return ApiResult<GetPackageTripByIdDto>.Created(packageTripResult.Value!);
        }

      

        [HttpPut("{Id}")]
        public async Task<ApiResult<string>> UpdatePackageTrip(int Id , [FromForm] UpdatePackageTripDto updatePackageTrip)
        {
            var packageTripResult = await _packageTripService.UpdateAsync(Id, updatePackageTrip);
            if (!packageTripResult.IsSuccess)
                return this.ToApiResult<string>(packageTripResult);
            return ApiResult<string>.Ok("Success Updated");

        }
      
        [HttpDelete("{Id}")]
        public async Task<ApiResult<string>> DeletePackageTrip(int Id)
        {
            var packageTripResult = await _packageTripService.DeleteAsync(Id);
            if (!packageTripResult.IsSuccess)
                return this.ToApiResult<string>(packageTripResult);
            return ApiResult<string>.Ok("Success Delete");
        }

        [HttpPost("Destination")]
        public async Task<ApiResult<GetPackageTripDestinationsDto>> AddPackageTripDestination(AddPackageTripDestinationDto packageTripDestination)
        {
            var packageTripDestinationResult = await _packageTripDestinationService.CreateAsync(packageTripDestination);
            if (!packageTripDestinationResult.IsSuccess)
            {
                return this.ToApiResult(packageTripDestinationResult);
            }
            return ApiResult<GetPackageTripDestinationsDto>.Created(packageTripDestinationResult.Value!);
        }
        //[HttpPut("Destination")]
        //public async Task<ApiResult<string>> UpdatePackageTripDestination([FromForm] UpdatePackageTripDestinationDto updatePackageTripDestination)
        //{
        //    var packageTripDestinationResult = await _packageTripDestinationService.UpdateAsync(0, updatePackageTripDestination);
        //    if (!packageTripDestinationResult.IsSuccess)
        //        return this.ToApiResult<string>(packageTripDestinationResult);
        //    return ApiResult<string>.Ok("Success Updated");

        //}
      
        [HttpDelete("{packageTripId}/Destination/{destinationId}")]
        public async Task<ApiResult<string>> DeletePackageTripDestination(int packageTripId, int destinationId)
        {
            var packageTripDestinationResult = await _packageTripDestinationService.DeletePackageTripDestinationAsync(packageTripId, destinationId);
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
            var packageTripDestinationActivityResult = await _packageTripDestinationActivityService.DeletePackageTripDestinationActivity(packageTripId, destinationId, activityId);

            if (!packageTripDestinationActivityResult.IsSuccess)
            {
                return this.ToApiResult<string>(packageTripDestinationActivityResult);
            }
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
        [HttpGet("{PackageTripId}/Destinations/Activities/Dates")]
        public async Task<ApiResult<GetPackageTripDestinationsActivitiesDatesDto>> GetPackageTripDestinationsActivitiesDates(int PackageTripId , enPackageTripDataStatusDto status)
        {
            var packageTripsResult = await _packageTripService.GetPackageTripDestinationsActivitiesDates(PackageTripId, status);
            if (!packageTripsResult.IsSuccess)
                return this.ToApiResult(packageTripsResult);
            return ApiResult<GetPackageTripDestinationsActivitiesDatesDto>.Ok(packageTripsResult.Value!);
        }

    }

}
