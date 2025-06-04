using AutoMapper;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Activity.Commands;
using TripAgency.Service.Feature.Activity.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implemetations
{
    public class ActivityService : GenericService<Activity, GetActivityByIdDto, GetActivitiesDto, AddActivityDto, UpdateActivityDto>, IActivityService
    {
        private IActivityRepositoryAsync _activityRepository { get; set; }
        public IMapper _mapper { get; }

        public ActivityService(IActivityRepositoryAsync activityRepository,
                           IMapper mapper
                           ) : base(activityRepository, mapper)
        {
            _activityRepository = activityRepository;
            _mapper = mapper;
        }
        public async Task<Result<GetActivityByIdDto>> GetActivityByNameAsync(string name)
        {
            var activity = await _activityRepository.GetActivityByName(name);
            if (activity is null)
                return Result<GetActivityByIdDto>.NotFound($"Not Found Activity with Name : {name}");
            var activityResult = _mapper.Map<GetActivityByIdDto>(activity);
            return Result<GetActivityByIdDto>.Success(activityResult);

        }

    }
}
