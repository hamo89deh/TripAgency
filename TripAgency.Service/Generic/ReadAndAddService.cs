using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Service.Generic
{
    public class ReadAndAddService<T, GetByIdDto, GetALlDto, AddDto> : IReadService<T, GetByIdDto, GetALlDto>, IAddService<T, AddDto, GetByIdDto>
    {
        public ReadAndAddService(IGenericRepositoryAsync<T> repositoryAsync, IMapper mapper)
        {
            _repoAsync = repositoryAsync;
            _mapper = mapper;
        }

        private IGenericRepositoryAsync<T> _repoAsync { get; }
        private IMapper _mapper { get; }

        public async Task<Result<IEnumerable<GetALlDto>>> GetAllAsync()
        {
            var entities = await _repoAsync.GetTableNoTracking().ToListAsync();
            if (entities.Count == 0)
                return Result<IEnumerable<GetALlDto>>.NotFound("Not Found Result");
            var citiesResult = _mapper.Map<List<GetALlDto>>(entities);
            return Result<IEnumerable<GetALlDto>>.Success(citiesResult);
        }

        public async Task<Result<GetByIdDto>> GetByIdAsync(int id)
        {
            var entity = await _repoAsync.GetByIdAsync(id);
            if (entity is null)
                return Result<GetByIdDto>.NotFound($"Not Found {typeof(T).Name} with Id : {id}");
            var entityResult = _mapper.Map<GetByIdDto>(entity);
            return Result<GetByIdDto>.Success(entityResult);
        }
        public virtual async Task<Result<GetByIdDto>> CreateAsync(AddDto AddDto)
        {
            var mapAddEntity = _mapper.Map<T>(AddDto);
            await _repoAsync.AddAsync(mapAddEntity);
            var resultEntity = _mapper.Map<GetByIdDto>(mapAddEntity);
            return Result<GetByIdDto>.Success(resultEntity);
        }

    }


}
