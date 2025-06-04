using AutoMapper;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Trip.Commands;
using TripAgency.Service.Feature.Trip.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implemetations
{
    public class TripService : GenericService<Trip, GetTripByIdDto, GetTripsDto, AddTripDto, UpdateTripDto>, ITripService
    {
        private ITripRepositoryAsync _tripRepository { get; set; }
        private ITypeTripRepositoryAsync _typeTripRepository { get; set; }
        public IMapper _mapper { get; }

        public TripService(ITripRepositoryAsync tripRepository,
                           IMapper mapper,
                           ITypeTripRepositoryAsync typeTripRepository) : base(tripRepository, mapper)
        {
            _tripRepository = tripRepository;
            _mapper = mapper;
            _typeTripRepository = typeTripRepository;
        }
        public async Task<Result<GetTripByIdDto>> GetTripByNameAsync(string name)
        {
            var trip = await _tripRepository.GetTripByName(name);
            if (trip is null)
                return Result<GetTripByIdDto>.NotFound($"Not Found Trip with Name : {name}");
            var tripResult = _mapper.Map<GetTripByIdDto>(trip);
            return Result<GetTripByIdDto>.Success(tripResult);

        }
    }
}
