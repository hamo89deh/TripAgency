using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Commands;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implemetations
{
    public class PackageTripDestinationActivityService : WriteService<PackageTripDestinationActivity, AddPackageTripDestinationActivityDto, UpdatePackageTripDestinationActivityDto, GetPackageTripDestinationActivitiesDto>, IPackageTripDestinationActivityService
    {
        private IActivityRepositoryAsync _activityRepositoryAsync { get; set; }
        public IDestinationActivityRepositoryAsync _destinationActivityRepositoryAsync { get; }
        private ITripDateRepositoryAsync _tripDateRepositoryAsync { get; }
        private IPackageTripDestinationRepositoryAsync _packageTripDestinationRepoAsync { get; set; }
        public IDestinationRepositoryAsync _destinationRepoAsync { get; }
        public IPackageTripRepositoryAsync _packageTripRepoAsync { get; }
        public IMapper _mapper { get; }
        public IPackageTripDestinationActivityRepositoryAsync _packageTripDestinationActivityRepoAsync { get; }
        public PackageTripDestinationActivityService(IPackageTripDestinationRepositoryAsync packageTripDestinationRepoAsync,
                                                     IPackageTripDestinationActivityRepositoryAsync packageTripDestinationActivityRepoAsync,
                                                     IDestinationRepositoryAsync destinationRepoAsync,
                                                     IPackageTripRepositoryAsync packageTripRepoAsync,
                                                     IMapper mapper,
                                                     IActivityRepositoryAsync activityRepositoryAsync,
                                                     IDestinationActivityRepositoryAsync destinationActivityRepositoryAsync,
                                                     ITripDateRepositoryAsync tripDateRepositoryAsync) : base(packageTripDestinationActivityRepoAsync, mapper)
        {
            _packageTripDestinationRepoAsync = packageTripDestinationRepoAsync;
            _destinationRepoAsync = destinationRepoAsync;
            _packageTripRepoAsync = packageTripRepoAsync;
            _mapper = mapper;
            _packageTripDestinationActivityRepoAsync = packageTripDestinationActivityRepoAsync;
            _activityRepositoryAsync = activityRepositoryAsync;
            _destinationActivityRepositoryAsync = destinationActivityRepositoryAsync;
            _tripDateRepositoryAsync = tripDateRepositoryAsync;
        }
        public override async Task<Result<GetPackageTripDestinationActivitiesDto>> CreateAsync(AddPackageTripDestinationActivityDto AddDto)
        {
            var PackageTrip = await _packageTripRepoAsync.GetByIdAsync(AddDto.PackageTripId);
            if (PackageTrip is null)
            {
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"Not Found PackageTrip With Id : {AddDto.PackageTripId}");
            }
            var Destination = await _destinationRepoAsync.GetByIdAsync(AddDto.DestinationId);
            if (Destination is null)
            {
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"Not Found Destination With Id : {AddDto.DestinationId}");
            }
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                               .Where(x => x.PackageTripId == AddDto.PackageTripId
                                                                                                    && x.DestinationId == AddDto.DestinationId)
                                                                               .Include(pd => pd.PackageTripDestinationActivities)
                                                                               .FirstOrDefaultAsync();

            if (PackageTripDestination is null)
            {
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"Not Found PackageTrip With Id : {AddDto.PackageTripId} and Destination with Id {AddDto.DestinationId}");

            }

            var requestAcivityIds = AddDto.ActivitiesDtos.Select(d => d.ActivityId)
                                                          .Distinct()
                                                          .ToList();

            if (requestAcivityIds.Count() != AddDto.ActivitiesDtos.Count())
            {
                return Result<GetPackageTripDestinationActivitiesDto>.BadRequest($"Duplicate Acitvites Ids found in the request. ");
            }

            var existActivites = await _activityRepositoryAsync.GetTableNoTracking()
                                                               .Where(a => requestAcivityIds.Contains(a.Id))
                                                               .ToListAsync();

            if (requestAcivityIds.Count() != existActivites.Count())
            {
                var notFoundActivitesIds = requestAcivityIds.Except(existActivites.Select(d => d.Id));
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"One or More from Activities Not found ,Missing Activity Ids : {string.Join(',', notFoundActivitesIds)} ");
            }

            var destinationActivities = await _destinationActivityRepositoryAsync.GetTableNoTracking()
                                                                       .Where(da => da.DestinationId == PackageTripDestination.DestinationId
                                                                                  && existActivites.Select(ea => ea.Id).Contains(da.ActivityId))
                                                                       .Include(d => d.Activity)
                                                                       .ToListAsync();

            if (destinationActivities.Count() != existActivites.Count())
            {
                var notFoundDestinationActivites = existActivites.Select(a => a.Id)
                                                                 .Except(destinationActivities.Select(d => d.Id));

                return Result<GetPackageTripDestinationActivitiesDto>.BadRequest($"The Destination With Id : {PackageTripDestination.DestinationId} Not Contain Activity with id {string.Join(',', notFoundDestinationActivites)}");
            }

            var PackageTripDestinationActivitiesToAdd = new List<PackageTripDestinationActivity>();

            foreach (var activity in AddDto.ActivitiesDtos)
            {
                // Check if the trip already has this Activity to avoid duplicates
                if (!PackageTripDestination.PackageTripDestinationActivities.Any(td => td.ActivityId == activity.ActivityId))
                {
                    PackageTripDestinationActivitiesToAdd.Add(new PackageTripDestinationActivity
                    {
                        ActivityId = activity.ActivityId,
                        Description = activity.Description,
                        OrderActivity = activity.OrderActivity,
                        Duration = activity.Duration,
                        StartTime = activity.StartTime,
                        EndTime = activity.EndTime,
                        Price = activity.Price,
                        PackageTripDestinationId = PackageTripDestination.Id

                    });
                }
            }
            GetPackageTripDestinationActivitiesDto resultDto;


            if (PackageTripDestinationActivitiesToAdd.Any())
            {
                await _packageTripDestinationActivityRepoAsync.AddRangeAsync(PackageTripDestinationActivitiesToAdd);



                resultDto = new GetPackageTripDestinationActivitiesDto()
                {
                    PackageTripId = PackageTripDestination.PackageTripId,
                    DestinationId = PackageTripDestination.DestinationId,
                    ActivitiesDtos = destinationActivities.Select(da => new PackageTripDestinationActivitiesDto
                    {
                        ActivityId = da.Activity.Id,
                        Description = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.Description,
                        // Name =da.Activity.Name,
                        Price = da.Activity.Price,
                        Duration = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.Duration,
                        StartTime = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.StartTime,
                        EndTime = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.EndTime,
                        OrderActivity = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.OrderActivity
                    })
                };
            }
            else
            {
                resultDto = new GetPackageTripDestinationActivitiesDto()
                {
                    ActivitiesDtos = [],

                    PackageTripId = PackageTripDestination.PackageTripId,
                    DestinationId = PackageTripDestination.DestinationId,
                };
            }

            return Result<GetPackageTripDestinationActivitiesDto>.Success(resultDto);
        }
        public override async Task<Result> UpdateAsync(int id, UpdatePackageTripDestinationActivityDto UpdateDto)
        {
            var PackageTrip = await _packageTripRepoAsync.GetByIdAsync(UpdateDto.PackageTripId);
            if (PackageTrip is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id : {UpdateDto.PackageTripId}");
            }

            var Destination = await _destinationRepoAsync.GetByIdAsync(UpdateDto.DestinationId);
            if (Destination is null)
            {
                return Result.NotFound($"Not Found Destination With Id : {UpdateDto.DestinationId}");
            }
            var Activity = await _activityRepositoryAsync.GetByIdAsync(UpdateDto.ActivityId);
            if (Activity is null)
            {
                return Result.NotFound($"Not Found Activity With Id : {UpdateDto.ActivityId}");
            }


            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                                .FirstOrDefaultAsync(x => x.PackageTripId == UpdateDto.PackageTripId
                                                                                                  && x.DestinationId == UpdateDto.DestinationId);

            if (PackageTripDestination is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id : {UpdateDto.PackageTripId} and Destination with Id {UpdateDto.DestinationId}");

            }

            var PackageTripDestinationActivity = await _packageTripDestinationActivityRepoAsync.GetTableNoTracking()
                                                                                               .FirstOrDefaultAsync(p => p.PackageTripDestinationId == PackageTripDestination.Id
                                                                                                                     && p.ActivityId == UpdateDto.ActivityId);
            if (PackageTripDestinationActivity is null)
            {
                return Result.NotFound($"Not Found PackageTrip with id {UpdateDto.PackageTripId} Destination {UpdateDto.DestinationId} Activity With Id : {UpdateDto.ActivityId}");

            }


            var TripDate = await _tripDateRepositoryAsync.GetTableNoTracking()
                                                         .Where(t => t.PackageTripId == PackageTripDestination.PackageTripId)
                                                         .FirstOrDefaultAsync();
            if (TripDate is not null)
            {
                return Result.BadRequest($"You Cann't Update PackageTrip Activity Because you have TripDate with Id: {TripDate.Id}");

            }
            return await base.UpdateAsync(PackageTripDestinationActivity.Id, UpdateDto);
        }

        public override async Task<Result> DeleteAsync(int id)
        {
            var PackageTripDestinationActivity = await _packageTripDestinationActivityRepoAsync.GetTableNoTracking()
                                                                                               .Where(x => x.Id == id)
                                                                                               .Include(x => x.PackageTripDestination)
                                                                                               .FirstOrDefaultAsync();
            if (PackageTripDestinationActivity is null)
            {
                return Result.NotFound($"Not Found PackageTripDestinationActivity With Id : {id}");

            }

            var TripDate = await _tripDateRepositoryAsync.GetTableNoTracking()
                                                          .Where(t => t.PackageTripId == PackageTripDestinationActivity.PackageTripDestination.PackageTripId)
                                                          .FirstOrDefaultAsync();
            if (TripDate is not null)
            {
                return Result.BadRequest($"Cann't Delete PackageTripDestinationActivity Because you have TripDate :{TripDate.Id} Connected before with PackageTrip: {PackageTripDestinationActivity.PackageTripDestination.PackageTripId}");

            }
            await _packageTripDestinationActivityRepoAsync.DeleteAsync(PackageTripDestinationActivity);
            return Result.Success();
        }

        public async Task<Result<GetPackageTripDestinationActivityByIdDto>> GetPackageTripDestinationActivity(int PackageTripId, int DestinationId, int ActivityId)
        {
            var packageTrip = await _packageTripRepoAsync.GetByIdAsync(PackageTripId);
            if (packageTrip is null)
            {
                return Result<GetPackageTripDestinationActivityByIdDto>.NotFound($"Not Found PackageTrip With id : {PackageTripId}");
            }
            var Destination = await _destinationRepoAsync.GetByIdAsync(DestinationId);
            if (Destination is null)
            {
                return Result<GetPackageTripDestinationActivityByIdDto>.NotFound($"Not Found Destination With Id : {DestinationId}");
            }
            var Activity = await _activityRepositoryAsync.GetByIdAsync(ActivityId);
            if (Activity is null)
            {
                return Result<GetPackageTripDestinationActivityByIdDto>.NotFound($"Not Found Activity With Id : {ActivityId}");
            }
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                                 .FirstOrDefaultAsync(x => x.PackageTripId == PackageTripId
                                                                                                   && x.DestinationId == DestinationId);

            if (PackageTripDestination is null)
            {
                return Result<GetPackageTripDestinationActivityByIdDto>.NotFound($"Not Found PackageTrip With Id : {PackageTripId} and Destination with Id {DestinationId}");

            }
            var PackageTripDestinationActivity = await _packageTripDestinationActivityRepoAsync.GetTableNoTracking()
                                                                                              .FirstOrDefaultAsync(p => p.PackageTripDestinationId == PackageTripDestination.Id
                                                                                                                    && p.ActivityId == ActivityId);
            if (PackageTripDestinationActivity is null)
            {
                return Result<GetPackageTripDestinationActivityByIdDto>.NotFound($"Not Found PackageTrip with id {PackageTripId} Destination {DestinationId} Activity With Id : {ActivityId}");

            }
            var resultDto = new GetPackageTripDestinationActivityByIdDto
            {
                ActivityId = PackageTripDestinationActivity.ActivityId,
                DestinationId = PackageTripDestination.DestinationId,
                PackageTripId = PackageTripDestination.PackageTripId,
                Description = PackageTripDestinationActivity.Description,
                Duration = PackageTripDestinationActivity.Duration,
                EndTime = PackageTripDestinationActivity.EndTime,
                StartTime = PackageTripDestinationActivity.StartTime,
                Id = PackageTripDestinationActivity.Id,
                OrderActivity = PackageTripDestinationActivity.OrderActivity,
                Price = PackageTripDestinationActivity.Price

            };
            return Result<GetPackageTripDestinationActivityByIdDto>.Success(resultDto);
        }
    }
}
