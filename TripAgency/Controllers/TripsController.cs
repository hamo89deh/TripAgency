using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MimeKit.Tnef;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Data.Helping;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Feature.Trip.Commands;
using TripAgency.Service.Feature.Trip.Queries;
using TripAgency.Service.Implementations;

namespace TripAgency.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class TripsController : ControllerBase
    {
        public TripsController(ITripService tripService, IMapper mapper)
        {
            _tripService = tripService;
            _mapper = mapper;
        }

        public ITripService _tripService { get; }
        public IMapper _mapper { get; }

        [HttpGet("ForUser")]
        public async Task<ApiResult<IEnumerable<GetTripsDto>>> GetTripsForUser()
        {
            var tripsResult = await _tripService.GetTripsForUsersAsync();
            if (!tripsResult.IsSuccess)
                return this.ToApiResult(tripsResult);
            return ApiResult<IEnumerable<GetTripsDto>>.Ok(tripsResult.Value!);
        }
        [HttpGet("Pagination/ForUser")]
        public async Task<ApiResult<PaginatedResult<GetTripsDto>>> GetTripsPagintionForUser(string? search , int pageNumber=1 ,int pageSize=10)
        {
            var tripsResult = await _tripService.GetTripsPaginationForUsersAsync(search,pageNumber,pageSize);
            if (!tripsResult.IsSuccess)
                return this.ToApiResult(tripsResult);
            return ApiResult<PaginatedResult<GetTripsDto>>.Ok(tripsResult.Value!);
        }
        [HttpGet("{TripId}/Pagination/PackageTrips")]
        public async Task<ApiResult<PaginatedResult<PackageTripForTripDto>>> GetPackagesPaginationForTrip(int TripId,string? search, int pageNumber = 1, int pageSize = 10)
        {
            var tripsResult = await _tripService.GetPackagesPaginationForTripAsync(TripId,search, pageNumber, pageSize);
            if (!tripsResult.IsSuccess)
                return this.ToApiResult(tripsResult);
            return ApiResult<PaginatedResult<PackageTripForTripDto>>.Ok(tripsResult.Value!);
        }
        [HttpGet("{TripId}/PackageTrips")]
        public async Task<ApiResult<GetPackageTripsForTripDto>> GetPackagesForTrip(int TripId)
        {
            var packageTripsResult = await _tripService.GetPackagesForTripAsync(TripId);
            if (!packageTripsResult.IsSuccess)
                return this.ToApiResult<GetPackageTripsForTripDto>(packageTripsResult);
            return ApiResult<GetPackageTripsForTripDto>.Ok(packageTripsResult.Value!);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]

        public async Task<ApiResult<IEnumerable<GetTripsDto>>> GetTrips()
        {
            var tripsResult = await _tripService.GetAllAsync();
            if (!tripsResult.IsSuccess)
                return this.ToApiResult(tripsResult);
            return ApiResult<IEnumerable<GetTripsDto>>.Ok(tripsResult.Value!);
        }
        [HttpGet("{Id}")]
        public async Task<ApiResult<GetTripByIdDto>> GetTripById(int Id)
        {
            var tripResult = await _tripService.GetByIdAsync(Id);
            if (!tripResult.IsSuccess)
                return this.ToApiResult(tripResult);
            return ApiResult<GetTripByIdDto>.Ok(tripResult.Value!);
        }
        [HttpGet("{Id}/Destinations")]
        [Authorize(Roles = "Admin")]

        public async Task<ApiResult<GetTripDestinationsDto>> GetTripDestinationsById(int Id)
        {
            var tripResult = await _tripService.GetTripDestinationsById(Id);
            if (!tripResult.IsSuccess)
                return this.ToApiResult(tripResult);
            return ApiResult<GetTripDestinationsDto>.Ok(tripResult.Value!);
        }

        [HttpGet("Name/{name}")]
        public async Task<ApiResult<GetTripByIdDto>> GetTripByName(string name)
        {
            var tripResult = await _tripService.GetTripByNameAsync(name);
            if (!tripResult.IsSuccess)
                return this.ToApiResult(tripResult);
            return ApiResult<GetTripByIdDto>.Ok(tripResult.Value!);
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]

        public async Task<ApiResult<GetTripByIdDto>> AddTrip([FromForm] AddTripDto trip)
        {
            var tripResult = await _tripService.CreateAsync(trip);
            if (!tripResult.IsSuccess)
            {
                return this.ToApiResult(tripResult);
            }
            return ApiResult<GetTripByIdDto>.Created(tripResult.Value!);
        }
        [HttpPost("Destination")]
        [Authorize(Roles = "Admin")]

        public async Task<ApiResult<GetTripDestinationsDto>> AddTripDestination(AddTripDestinationsDto addTripDestinationsDto)
        {
            var tripResult = await _tripService.AddTripDestinations(addTripDestinationsDto);
            if (!tripResult.IsSuccess)
            {
                return this.ToApiResult(tripResult);
            }
            return ApiResult<GetTripDestinationsDto>.Created(tripResult.Value!);
        }
        [HttpPut("{Id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ApiResult<string>> UpdateTrip(int Id, [FromForm] UpdateTripDto updateTrip)
        {
            var tripResult = await _tripService.UpdateAsync(Id, updateTrip);
            if (!tripResult.IsSuccess)
                return this.ToApiResult<string>(tripResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]

        public async Task<ApiResult<string>> DeleteTrip(int Id)
        {
            var tripResult = await _tripService.DeleteAsync(Id);
            if (!tripResult.IsSuccess)
                return this.ToApiResult<string>(tripResult);
            return ApiResult<string>.Ok("Success Delete");
        }
        [HttpDelete("{TripId}/Destination/{DestinationId}")]
        [Authorize(Roles = "Admin")]

        public async Task<ApiResult<string>> DeleteTripDestination(int TripId , int DestinationId)
        {
            var tripResult = await _tripService.DeleteTripDestination(TripId, DestinationId);
            if (!tripResult.IsSuccess)
                return this.ToApiResult<string>(tripResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }
}
