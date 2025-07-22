using AutoMapper;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Service.Generic
{
    public class WriteService<T, AddDto, UpdateDto,GetByIdDto> : IWriteService<T, AddDto, UpdateDto , GetByIdDto>
    {
        public WriteService(IGenericRepositoryAsync<T> repositoryAsync, IMapper mapper)
        {
            _repoAsync = repositoryAsync;
            _mapper = mapper;
        }

        private IGenericRepositoryAsync<T> _repoAsync { get; }
        private IMapper _mapper { get; }

        public virtual async Task<Result> UpdateAsync(int id, UpdateDto UpdateDto)
        {
            var entity = await _repoAsync.GetByIdAsync(id);
            if (entity is null)
                return Result.NotFound($"Not Found {typeof(T).Name} with Id : {id}");
            var mapeEntity = _mapper.Map(UpdateDto, entity);
            await _repoAsync.UpdateAsync(entity);
            return Result.Success();
        }

        public virtual async Task<Result<GetByIdDto>> CreateAsync(AddDto AddDto)
        {
            var mapAddEntity = _mapper.Map<T>(AddDto);
            await _repoAsync.AddAsync(mapAddEntity);
            var resultEntity = _mapper.Map<GetByIdDto>(mapAddEntity);
            return Result<GetByIdDto>.Success(resultEntity);
        }

     
    }  
    public class DeleteService<T> : IDeleteService<T>
    {
        public DeleteService(IGenericRepositoryAsync<T> repositoryAsync, IMapper mapper)
        {
            _repoAsync = repositoryAsync;
            _mapper = mapper;
        }

        private IGenericRepositoryAsync<T> _repoAsync { get; }
        private IMapper _mapper { get; }

       
        public virtual async Task<Result> DeleteAsync(int id)
        {
            var entity = await _repoAsync.GetByIdAsync(id);
            if (entity is null)
                return Result.NotFound($"Not Found {typeof(T).Name} Id : {id}");
            await _repoAsync.DeleteAsync(entity);
            return Result.Success();
        }   
    }

    public class WriteAndDeleteService<T, AddDto, UpdateDto, GetByIdDto> : IWriteService<T, AddDto, UpdateDto, GetByIdDto> , IDeleteService<T>
    {
        public WriteAndDeleteService(IGenericRepositoryAsync<T> repositoryAsync, IMapper mapper)
        {
            _repoAsync = repositoryAsync;
            _mapper = mapper;
        }

        private IGenericRepositoryAsync<T> _repoAsync { get; }
        private IMapper _mapper { get; }

        public virtual async Task<Result> UpdateAsync(int id, UpdateDto UpdateDto)
        {
            var entity = await _repoAsync.GetByIdAsync(id);
            if (entity is null)
                return Result.NotFound($"Not Found {typeof(T).Name} with Id : {id}");
            var mapeEntity = _mapper.Map(UpdateDto, entity);
            await _repoAsync.UpdateAsync(entity);
            return Result.Success();
        }

        public virtual async Task<Result<GetByIdDto>> CreateAsync(AddDto AddDto)
        {
            var mapAddEntity = _mapper.Map<T>(AddDto);
            await _repoAsync.AddAsync(mapAddEntity);
            var resultEntity = _mapper.Map<GetByIdDto>(mapAddEntity);
            return Result<GetByIdDto>.Success(resultEntity);
        }

        public virtual async Task<Result> DeleteAsync(int id)
        {
            var entity = await _repoAsync.GetByIdAsync(id);
            if (entity is null)
                return Result.NotFound($"Not Found {typeof(T).Name} Id : {id}");
            await _repoAsync.DeleteAsync(entity);
            return Result.Success();
        }
    }
}
