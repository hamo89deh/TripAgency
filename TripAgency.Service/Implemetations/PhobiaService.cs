using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
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
        public override async Task<Result> DeleteAsync(int id)
        {
            var phobia = await _phobiaRepositoryAsync.GetTableNoTracking()
                                                    .Where(x => x.Id == id)
                                                    .Include(x => x.ActivityPhobias)
                                                    .Include(x => x.UserPhobias)
                                                    .FirstOrDefaultAsync();
            if(phobia is null)
            {
                return Result.NotFound($"Not Found Phobia With id : {id}");
            }
            if (phobia.ActivityPhobias.Any())
            {
                return Result.BadRequest($"Cannot delete Phobia with associated activities.");
            }
            return await base.DeleteAsync(id);    
        }
    }
}
