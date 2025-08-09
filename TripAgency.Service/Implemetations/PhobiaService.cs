using AutoMapper;
using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Phobia.Commands;
using TripAgency.Service.Feature.Phobia.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class PhobiaService : GenericService<Phobia, GetPhobiaByIdDto, GetPhobiasDto, AddPhobiaDto, UpdatePhobiaDto>, IPhobiaService
    {
        public  IPhobiaRepositoryAsync _phobiaRepositoryAsync { get; set; }
        public PhobiaService(IPhobiaRepositoryAsync phobiaRepositoryAsync, IMapper mapper) : base(phobiaRepositoryAsync, mapper)
        {
            _phobiaRepositoryAsync = phobiaRepositoryAsync;
        }
    }
}
