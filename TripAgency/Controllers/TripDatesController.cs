using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TripDate.Commands;
using TripAgency.Service.Feature.TripDate.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripDatesController : ControllerBase
    {
        public TripDatesController(ITripDateService tripDateService, IMapper mapper)
        {
            _tripDateService = tripDateService;
            _mapper = mapper;
        }

        public ITripDateService _tripDateService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetTripDatesDto>>> GetTripDates()
        {
            var tripDatesResult = await _tripDateService.GetAllAsync();
            if (!tripDatesResult.IsSuccess)
                return this.ToApiResult(tripDatesResult);
            return ApiResult<IEnumerable<GetTripDatesDto>>.Ok(tripDatesResult.Value!);
        }
        [HttpGet("{id}")]
        public async Task<ApiResult<GetTripDateByIdDto>> GetTripDateById(int id)
        {
            var tripDateResult = await _tripDateService.GetByIdAsync(id);
            if (!tripDateResult.IsSuccess)
                return this.ToApiResult(tripDateResult);
            return ApiResult<GetTripDateByIdDto>.Ok(tripDateResult.Value!);
        }

      
        [HttpPost]
        public async Task<ApiResult<GetTripDateByIdDto>> AddTripDate(AddTripDateDto tripDate)
        {
            var tripDateResult = await _tripDateService.CreateAsync(tripDate);
            if (!tripDateResult.IsSuccess)
            {
                return this.ToApiResult(tripDateResult);
            }
            return ApiResult<GetTripDateByIdDto>.Created(tripDateResult.Value!);
        }       
    }

}
