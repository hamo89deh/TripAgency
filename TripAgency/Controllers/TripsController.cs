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
        [HttpGet("{id}")]
        public async Task<ApiResult<GetTripByIdDto>> GetTripById(int id)
        {
            var tripResult = await _tripService.GetByIdAsync(id);
            if (!tripResult.IsSuccess)
                return this.ToApiResult(tripResult);
            return ApiResult<GetTripByIdDto>.Ok(tripResult.Value!);
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
        [HttpPut]
        public async Task<ApiResult<string>> UpdateTrip(UpdateTripDto updateTrip)
        {
            var tripResult = await _tripService.UpdateAsync(updateTrip.Id, updateTrip);
            if (!tripResult.IsSuccess)
                return this.ToApiResult<string>(tripResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteTrip(int id)
        {
            var tripResult = await _tripService.DeleteAsync(id);
            if (!tripResult.IsSuccess)
                return this.ToApiResult<string>(tripResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }
}
