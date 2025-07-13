using AutoMapper;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
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
        private IPackageTripDateRepositoryAsync _tripDateRepository { get; set; }
        public IMapper _mapper { get; }

        public BookingTripService(IBookingTripRepositoryAsync bookingTripRepository,
                                  IPackageTripDateRepositoryAsync tripDateRepository,
                                  IMapper mapper
                                 ) : base(bookingTripRepository, mapper)
        {
            _bookingTripRepository = bookingTripRepository;
            _tripDateRepository = tripDateRepository;
            _mapper = mapper;
        }
        public override async Task<Result<GetBookingTripByIdDto>> CreateAsync(AddBookingTripDto AddDto)
        {
            var TripDate = await _tripDateRepository.GetByIdAsync(AddDto.TripDateId);
            if( TripDate is null )
            {
                return Result<GetBookingTripByIdDto>.NotFound($"Not Found TripDate With Id {AddDto.TripDateId}");
            }
            //ToDo Check userId
            return await base.CreateAsync(AddDto);
        }
        public override async Task<Result> UpdateAsync(int id, UpdateBookingTripDto UpdateDto)
        {
            var TripDate = await _tripDateRepository.GetByIdAsync(UpdateDto.TripDateId);
            if (TripDate is null)
            {
                return Result.NotFound($"Not Found TripDate With Id {UpdateDto.TripDateId}");
            }
            //ToDo Check userId
            return await base.UpdateAsync(id, UpdateDto);
        }
    }
}
