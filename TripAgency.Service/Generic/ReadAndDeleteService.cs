using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;
using TripAgency.Service.Feature.City.Command;
using TripAgency.Service.Feature.City.Queries;

namespace TripAgency.Service.Generic
{
    public class ReadAndDeleteService<T, GetByIdDto, GetALlDto> : IReadService<T, GetByIdDto, GetALlDto> , IDeleteService<T>
    {
        public ReadAndDeleteService( IGenericRepositoryAsync<T> repositoryAsync , IMapper mapper)
        {
            _repoAsync = repositoryAsync;
            _mapper = mapper;
        }

        private IGenericRepositoryAsync<T> _repoAsync { get; }
        private IMapper _mapper { get; }

        public async Task<Result> DeleteAsync(int id)
        {
            var entity = await _repoAsync.GetByIdAsync(id);
            if (entity is null)
                return Result.NotFound($"Not Found {typeof(T).Name} Id : {id}");
            await _repoAsync.DeleteAsync(entity);
            return Result.Success();
        }

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
    }


}
