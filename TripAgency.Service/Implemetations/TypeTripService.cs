using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.TypeTrip_Entity.Commands;
using TripAgency.Service.Feature.TypeTrip_Entity.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class TripTypeService : GenericService<TypeTrip, GetTypeTripByIdDto, GetTripTypesDto, AddTypeTripDto, UpdateTypeTripDto>, ITripTypeService
    {
        private ITripTypeRepositoryAsync _typeTripRepository { get; set; }
        public IMapper _mapper { get; }

        public TripTypeService(ITripTypeRepositoryAsync typeTripRepository,
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
        public override async Task<Result> DeleteAsync(int id)
        {
            var typeTrip = await _typeTripRepository.GetTableNoTracking()
                                                     .Where(x => x.Id == id)
                                                     .Include(x => x.PackageTripTypes)
                                                     .FirstOrDefaultAsync();
            if (typeTrip is null)
                return Result.NotFound($"Not Found TypeTrip with Id : {id}");

            if (typeTrip is null)
                return Result.BadRequest($"Cannot delete Type with associated Package Trip Types");
            return await base.DeleteAsync(id);    
        }

    }
}

