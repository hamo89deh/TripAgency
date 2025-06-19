using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.BookingTrip.Commands;
using TripAgency.Service.Feature.BookingTrip.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingTripsController : ControllerBase
    {
        public BookingTripsController(IBookingTripService bookingTripService, IMapper mapper)
        {
            _bookingTripService = bookingTripService;
            _mapper = mapper;
        }

        public IBookingTripService _bookingTripService { get; }
        public IMapper _mapper { get; }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<GetBookingTripsDto>>> GetBookingTrips()
        {
            var bookingTripsResult = await _bookingTripService.GetAllAsync();
            if (!bookingTripsResult.IsSuccess)
                return this.ToApiResult(bookingTripsResult);
            return ApiResult<IEnumerable<GetBookingTripsDto>>.Ok(bookingTripsResult.Value!);
        }
        [HttpGet("{id}")]
        public async Task<ApiResult<GetBookingTripByIdDto>> GetBookingTripById(int id)
        {
            var bookingTripResult = await _bookingTripService.GetByIdAsync(id);
            if (!bookingTripResult.IsSuccess)
                return this.ToApiResult(bookingTripResult);
            return ApiResult<GetBookingTripByIdDto>.Ok(bookingTripResult.Value!);
        }

        [HttpPost]
        public async Task<ApiResult<GetBookingTripByIdDto>> AddBookingTrip(AddBookingTripDto bookingTrip)
        {
            var bookingTripResult = await _bookingTripService.CreateAsync(bookingTrip);
            if (!bookingTripResult.IsSuccess)
            {
                return this.ToApiResult(bookingTripResult);
            }
            return ApiResult<GetBookingTripByIdDto>.Created(bookingTripResult.Value!);
        }
        [HttpPut]
        public async Task<ApiResult<string>> UpdateBookingTrip(UpdateBookingTripDto updateBookingTrip)
        {
            var bookingTripResult = await _bookingTripService.UpdateAsync(updateBookingTrip.Id, updateBookingTrip);
            if (!bookingTripResult.IsSuccess)
                return this.ToApiResult<string>(bookingTripResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteBookingTrip(int id)
        {
            var bookingTripResult = await _bookingTripService.DeleteAsync(id);
            if (!bookingTripResult.IsSuccess)
                return this.ToApiResult<string>(bookingTripResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }

}
