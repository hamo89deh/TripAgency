using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.BookingPassenger.Commands;
using TripAgency.Service.Feature.BookingPassenger.Queries;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingPassengersController : ControllerBase
    {
        public BookingPassengersController(IBookingPassengerService bookingPassengerService, IMapper mapper)
        {
            _bookingPassengerService = bookingPassengerService;
            _mapper = mapper;
        }

        public IBookingPassengerService _bookingPassengerService { get; }
        public IMapper _mapper { get; }

        [HttpGet("{BookingTripId}")]
        public async Task<ApiResult<IEnumerable<GetBookingPassengersDto>>> GetBookingPassengers(int BookingTripId)
        {
            var bookingPassengersResult = await _bookingPassengerService.GetBookingPassengers(BookingTripId);
            if (!bookingPassengersResult.IsSuccess)
                return this.ToApiResult(bookingPassengersResult);
            return ApiResult<IEnumerable<GetBookingPassengersDto>>.Ok(bookingPassengersResult.Value!);
        }
        [HttpPost("AddBookingPassengers")]
        public async Task<ApiResult<string>> AddBookingPassenger(AddBookingPassengersDto bookingPassengers)
        {
            var bookingPassengerResult = await _bookingPassengerService.AddBookingPassengers(bookingPassengers);
            if (!bookingPassengerResult.IsSuccess)
            {
                return this.ToApiResult<string>(bookingPassengerResult);
            }
            return ApiResult<string>.Created(bookingPassengerResult.Message!);
        }
        [HttpPost]
        public async Task<ApiResult<string>> AddBookingPassenger(AddBookingPassengerDto bookingPassenger)
        {
            var bookingPassengerResult = await _bookingPassengerService.AddBookingPassenger(bookingPassenger);
            if (!bookingPassengerResult.IsSuccess)
            {
                return this.ToApiResult<string>(bookingPassengerResult);
            }
            return ApiResult<string>.Created(bookingPassengerResult.Message!);
        }

        [HttpPut]
        public async Task<ApiResult<string>> UpdateBookingPassenger(UpdateBookingPassengerDto updateBookingPassenger)
        {
            var bookingPassengerResult = await _bookingPassengerService.UpdateBookingPassenger(updateBookingPassenger);
            if (!bookingPassengerResult.IsSuccess)
                return this.ToApiResult<string>(bookingPassengerResult);
            return ApiResult<string>.Ok(bookingPassengerResult.Message!);

        }
    
        [HttpDelete]
        public async Task<ApiResult<string>> DeleteBookingPassenger(int id)
        {
            var bookingPassengerResult = await _bookingPassengerService.DeleteBookingPassenger(id);
            if (!bookingPassengerResult.IsSuccess)
                return this.ToApiResult<string>(bookingPassengerResult);
            return ApiResult<string>.Ok("Success Delete");
        }
    }

}
