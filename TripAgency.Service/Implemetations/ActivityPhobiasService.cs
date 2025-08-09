using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.ActivityPhobia.Commands;
using TripAgency.Service.Feature.ActivityPhobia.Queries;
using TripAgency.Service.Feature.Phobia.Queries;

namespace TripAgency.Service.Implementations
{
    public class ActivityPhobiasService : IActivityPhobiasService
    {
        private IActivityRepositoryAsync _activityRepository { get; set; }
        public IMapper _mapper { get; }
        public IPhobiaRepositoryAsync _phobiaRepositoryAsync { get; }
        public IActivityPhobiasRepositoryAsync _activityPhobiasRepositoryAsync { get; }

        public ActivityPhobiasService(IActivityRepositoryAsync activityRepository,
                           IMapper mapper,
                           IPhobiaRepositoryAsync phobiaRepositoryAsync,
                           IActivityPhobiasRepositoryAsync activityPhobiasRepositoryAsync
                           )
        {
            _activityRepository = activityRepository;
            _mapper = mapper;
            _phobiaRepositoryAsync = phobiaRepositoryAsync;
            _activityPhobiasRepositoryAsync = activityPhobiasRepositoryAsync;
            ;
        }
        public async Task<Result> AddActivityPhobias(AddActivityPhobiasDto addActivityPhobiasDto)
        {
            var activity = await _activityRepository.GetByIdAsync(addActivityPhobiasDto.ActivityId);
            if (activity is null)
                return Result.NotFound($"Not Found Activity with Id : {addActivityPhobiasDto.ActivityId}");


            var DistinctPhobiasId = new HashSet<int>();
            var DublicatePhobiaId = new HashSet<int>();
            foreach (var id in addActivityPhobiasDto.Phobias)
                if (!DistinctPhobiasId.Add(id))
                    DublicatePhobiaId.Add(id);

            if (DublicatePhobiaId.Count() != 0)
            {
                return Result.BadRequest($"Duplicate Phobias Id : {string.Join(',', DublicatePhobiaId)}");
            }

            var existPhobias = await _phobiaRepositoryAsync.GetTableNoTracking()
                                                           .Where(a => DistinctPhobiasId.Contains(a.Id))
                                                           .ToListAsync();

            if (DistinctPhobiasId.Count() != existPhobias.Count())
            {
                var notFoundPhoniesId = DistinctPhobiasId.Except(existPhobias.Select(d => d.Id));
                return Result.NotFound($"One or More from Phonies Not found ,Missing Phonies Id : {string.Join(',', notFoundPhoniesId)} ");
            }
            var existActivityPhobia = await _activityPhobiasRepositoryAsync.GetTableNoTracking()
                                                                           .Where(x => x.ActivityId == activity.Id)
                                                                           .ToListAsync();

            var PrePhobia = existActivityPhobia.Where(x => DistinctPhobiasId.Contains(x.PhobiaId));
            if (PrePhobia.Count() != 0)
            {
                return Result.BadRequest($"the Phobia With Id : {string.Join(',', PrePhobia.Select(x => x.PhobiaId))} Adding Before");
            }
            foreach (var PhobiasId in DistinctPhobiasId)
            {
                await _activityPhobiasRepositoryAsync.AddAsync(new ActivityPhobias
                {
                    ActivityId = activity.Id,
                    PhobiaId = PhobiasId
                });
            };

            return Result.Success("Add Phobia To Activity");
        }

        public async Task<Result<GetActivityPhobiasDto>> GetActivityPhobiasAsync(int ActivityId)
        {
            var activity = await _activityRepository.GetTableNoTracking()
                                                          .Where(x => x.Id == ActivityId)
                                                          .Include(x => x.ActivityPhobias)
                                                          .ThenInclude(x => x.Phobia)
                                                          .FirstOrDefaultAsync();
            if (activity is null)
            {
                return Result<GetActivityPhobiasDto>.NotFound($"Not Found any Activity With id : {ActivityId}");

            }
            if (activity.ActivityPhobias.Count() == 0)
                return Result<GetActivityPhobiasDto>.NotFound($"Not Found any Phobias for Activity With Id : {activity.Id}");
            var result = new GetActivityPhobiasDto()
            {
                ActivityId = ActivityId,
                PhobiasDtos = new List<GetPhobiasDto>()
            };
            foreach (var activityPhobias in activity.ActivityPhobias)
            {
                result.PhobiasDtos.Add(new GetPhobiasDto
                {
                    Id = activityPhobias.PhobiaId,
                    Description = activityPhobias.Phobia.Description,
                    Name = activityPhobias.Phobia.Name
                });
            }
            return Result<GetActivityPhobiasDto>.Success(result);
        }

        public async Task<Result> DeleteActivityPhobia(int ActivityId, int PhobiaId)
        {
            var activity = await _activityRepository.GetTableNoTracking()
                                                    .FirstOrDefaultAsync(x => x.Id == ActivityId);
            if (activity is null)
            {
                return Result.NotFound($"Not Found Activity With Id :{ActivityId}");
            }
            var phobia = await _phobiaRepositoryAsync.GetTableNoTracking()
                                                     .FirstOrDefaultAsync(x => x.Id == PhobiaId);
            if (phobia is null)
            {
                return Result.NotFound($"Not Found Phobia With Id :{PhobiaId}");
            }
            var ActivityPhobia = await _activityPhobiasRepositoryAsync.GetTableNoTracking()
                                                                      .FirstOrDefaultAsync(x => x.ActivityId == ActivityId &&
                                                                                x.PhobiaId == PhobiaId)
                                                                      ;
            if (ActivityPhobia is null)
            {
                return Result.NotFound($"Not Found Activity With Id :{ActivityId} related With Phobia With Id :{PhobiaId}");
            }
            await _activityPhobiasRepositoryAsync.DeleteAsync(ActivityPhobia);
            return Result.Success("Deleted Success ");
        }
    }
}
