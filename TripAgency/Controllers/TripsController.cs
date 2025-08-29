using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Trip.Commands;
using TripAgency.Service.Feature.Trip.Queries;

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

        [HttpGet]
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
        public async Task<ApiResult<GetTripByIdDto>> AddTrip(AddTripDto trip)
        {
            var tripResult = await _tripService.CreateAsync(trip);
            if (!tripResult.IsSuccess)
            {
                return this.ToApiResult(tripResult);
            }
            return ApiResult<GetTripByIdDto>.Created(tripResult.Value!);
        }
        [HttpPost("Destination")]
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
        public async Task<ApiResult<string>> UpdateTrip(int Id,UpdateTripDto updateTrip)
        {
            var tripResult = await _tripService.UpdateAsync(Id, updateTrip);
            if (!tripResult.IsSuccess)
                return this.ToApiResult<string>(tripResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete("{Id}")]
        public async Task<ApiResult<string>> DeleteTrip(int Id)
        {
            var tripResult = await _tripService.DeleteAsync(Id);
            if (!tripResult.IsSuccess)
                return this.ToApiResult<string>(tripResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }
}
