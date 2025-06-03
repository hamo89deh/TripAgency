using AutoMapper;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TypeTrip_Entity.Commands;
using TripAgency.Service.Feature.TypeTrip_Entity.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implemetations
{
    public class TypeTripService : GenericService<TypeTrip, GetTypeTripByIdDto, GetTypeTripsDto, AddTypeTripDto, UpdateTypeTripDto>, ITypeTripService
    {
        private ITypeTripRepositoryAsync _typeTripRepository { get; set; }
        public IMapper _mapper { get; }

        public TypeTripService(ITypeTripRepositoryAsync typeTripRepository,
                           IMapper mapper
                           ) : base(typeTripRepository, mapper)
        {
            _typeTripRepository = typeTripRepository;
            _mapper = mapper;
        }
        public async Task<Result<GetTypeTripByIdDto>> GetTypeTripByNameAsync(string name)
        {
            var typeTrip = await _typeTripRepository.GetTypeTripByName(name);
            if (typeTrip is null)
                return Result<GetTypeTripByIdDto>.NotFound($"Not Found TypeTrip with Name : {name}");
            var tripResult = _mapper.Map<GetTypeTripByIdDto>(typeTrip);
            return Result<GetTypeTripByIdDto>.Success(tripResult);

        }

    }
}
