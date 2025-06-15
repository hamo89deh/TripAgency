using AutoMapper;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.BookingTrip.Commands;
using TripAgency.Service.Feature.BookingTrip.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class BookingTripService : GenericService<BookingTrip, GetBookingTripByIdDto, GetBookingTripsDto, AddBookingTripDto, UpdateBookingTripDto>, IBookingTripService
    {
        private IBookingTripRepositoryAsync _bookingTripRepository { get; set; }
        public IMapper _mapper { get; }

        public BookingTripService(IBookingTripRepositoryAsync bookingTripRepository,
                           IMapper mapper
                           ) : base(bookingTripRepository, mapper)
        {
            _bookingTripRepository = bookingTripRepository;
            _mapper = mapper;
        }
    }
}
