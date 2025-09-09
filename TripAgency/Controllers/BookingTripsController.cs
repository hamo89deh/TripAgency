using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TripAgency.Api.Extention;
using TripAgency.Bases;
using TripAgency.Data.Enums;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.BookingTrip.Commands;
using TripAgency.Service.Feature.BookingTrip.Queries;
using TripAgency.Service.Feature.Payment;
using TripAgency.Service.Implementations;

namespace TripAgency.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        [Authorize(Roles = "Admin")]
        public async Task<ApiResult<IEnumerable<GetBookingTripsDto>>> GetBookingTrips()
        {
            var bookingTripsResult = await _bookingTripService.GetAllAsync();
            if (!bookingTripsResult.IsSuccess)
                return this.ToApiResult(bookingTripsResult);
            return ApiResult<IEnumerable<GetBookingTripsDto>>.Ok(bookingTripsResult.Value!);
        }
        [HttpGet("ForUser")]
        [Authorize(Roles = "User")]
        public async Task<ApiResult<IEnumerable<GetBookingTripForUserDto>>> GetBookingsPackageTripForUserAsync(BookingStatus bookingStatus)
        {
            var bookingTripsResult = await _bookingTripService.GetBookingsPackageTripForUserAsync(bookingStatus);
            if (!bookingTripsResult.IsSuccess)
                return this.ToApiResult(bookingTripsResult);
            return ApiResult<IEnumerable<GetBookingTripForUserDto>>.Ok(bookingTripsResult.Value!);
        }
        [HttpGet("{bookingTripId}/ForUser")]
        [Authorize(Roles = "User")]
        public async Task<ApiResult<GetBookingTripForUserDto>> GetBookingTrips(int bookingTripId , BookingStatus bookingStatus)
        {
            var bookingTripsResult = await _bookingTripService.GetBookingPackageTripForUserAsync(bookingTripId, bookingStatus);
            if (!bookingTripsResult.IsSuccess)
                return this.ToApiResult(bookingTripsResult);
            return ApiResult<GetBookingTripForUserDto>.Ok(bookingTripsResult.Value!);
        }
        [HttpGet("{Id}")]
        public async Task<ApiResult<GetBookingTripByIdDto>> GetBookingTripById(int Id)
        {
            var bookingTripResult = await _bookingTripService.GetByIdAsync(Id);
            if (!bookingTripResult.IsSuccess)
                return this.ToApiResult(bookingTripResult);
            return ApiResult<GetBookingTripByIdDto>.Ok(bookingTripResult.Value!);
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ApiResult<PaymentInitiationResponseDto>> AddBookingTrip(AddBookingPackageTripDto bookingTrip)
        {
            var bookingTripResult = await _bookingTripService.InitiateBookingAndPaymentAsync(bookingTrip);
            if (!bookingTripResult.IsSuccess)
            {
                return this.ToApiResult(bookingTripResult);
            }
            return ApiResult<PaymentInitiationResponseDto>.Created(bookingTripResult.Value!);
        }
             
        [HttpPut("{Id}")]
        [Authorize(Roles = "User")]

        public async Task<ApiResult<string>> UpdateBookingTrip(int Id , UpdateBookingTripDto updateBookingTrip)
        {
            var bookingTripResult = await _bookingTripService.UpdateAsync(Id, updateBookingTrip);
            if (!bookingTripResult.IsSuccess)
                return this.ToApiResult<string>(bookingTripResult);
            return ApiResult<string>.Ok("Success Updated");

        }
        [HttpPut("CancellingBookingTrip/{Id}")]
        [Authorize(Roles = "User")]

        public async Task<ApiResult<string>> CansellingBookingTrip(int Id)
        {
            var bookingTripResult = await _bookingTripService.CancellingBookingAndRefundPayemntAsync(Id);
            if (!bookingTripResult.IsSuccess)
                return this.ToApiResult<string>(bookingTripResult);
            return ApiResult<string>.Ok("Success Delete");
        } 
    }

}
