using AutoMapper;
using System.Collections.Generic;
using TripAgency.Data.Entities;
using TripAgency.Data.Helping;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Activity.Commands;
using TripAgency.Service.Feature.Activity.Queries;
using TripAgency.Service.Generic;


namespace TripAgency.Service.Implementations
{
    public class ActivityService : GenericService<Activity, GetActivityByIdDto, GetActivitiesDto, AddActivityDto, UpdateActivityDto>, IActivityService
    {
        private object activityPagination;

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

        public async Task<Result<PaginatedResult<GetActivitiesDto>>> GetActivityPagination(string searchItem, Dictionary<string, string> filters, string sortColumn, string sortDirection, int PageNumber, int pageSize)
        {
            var activity = _activityRepository.GetTableNoTracking();
            activity.ApplySearch(searchItem, new string[] { "Name", "Description" });
            activity.ApplyFilter(filters);
            activity.ApplySorting(sortColumn, sortDirection);
            var activityMapping = activity.Select(item => new GetActivitiesDto
            {
                Id = item.Id,
                Description = item.Description,
                Name = item.Name,
                Price = item.Price
            });

            var activityResult = await activityMapping.ToPaginatedListAsync(PageNumber, pageSize);
            if (activityResult.Data.Count() == 0)
                return Result<PaginatedResult<GetActivitiesDto>>.Success(PaginatedResult<GetActivitiesDto>.Success(new(), 0, PageNumber, pageSize));
            return Result<PaginatedResult<GetActivitiesDto>>.Success(activityResult);
        }
    }

}
