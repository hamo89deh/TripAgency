using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Commands;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Implementations
{
    public class PackageTripDestinationActivityService : WriteService<PackageTripDestinationActivity, AddPackageTripDestinationActivityDto, UpdatePackageTripDestinationActivityDto, GetPackageTripDestinationActivitiesDto>, IPackageTripDestinationActivityService
    {
        private IActivityRepositoryAsync _activityRepositoryAsync { get; set; }
        public IDestinationActivityRepositoryAsync _destinationActivityRepositoryAsync { get; }
        private IPackageTripDateRepositoryAsync _pacakgeTripDateRepositoryAsync { get; }
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
                                                     IPackageTripDateRepositoryAsync tripDateRepositoryAsync) : base(packageTripDestinationActivityRepoAsync, mapper)
        {
            _packageTripDestinationRepoAsync = packageTripDestinationRepoAsync;
            _destinationRepoAsync = destinationRepoAsync;
            _packageTripRepoAsync = packageTripRepoAsync;
            _mapper = mapper;
            _packageTripDestinationActivityRepoAsync = packageTripDestinationActivityRepoAsync;
            _activityRepositoryAsync = activityRepositoryAsync;
            _destinationActivityRepositoryAsync = destinationActivityRepositoryAsync;
            _pacakgeTripDateRepositoryAsync = tripDateRepositoryAsync;
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

            var PackageTripDate = await _pacakgeTripDateRepositoryAsync.GetTableNoTracking()
                                                               .Where(t => t.PackageTripId == PackageTrip.Id)
                                                               .Take(2)
                                                               .ToListAsync();

            if (PackageTripDate.Any())
            {
                if (PackageTripDate.Count() != 1 || PackageTripDate[0].Status != Data.Enums.PackageTripDateStatus.Draft)
                {
                    return Result<GetPackageTripDestinationActivitiesDto>.BadRequest($"You Cann't Add Any New Activity For PackageTrip With Id {PackageTrip.Id} Because you have already Published it Before");
                }
            }
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetTableNoTracking()
                                                                             .Where(x => x.PackageTripId == AddDto.PackageTripId
                                                                                      && x.DestinationId == AddDto.DestinationId)
                                                                             .Include(pd => pd.PackageTripDestinationActivities)
                                                                             .FirstOrDefaultAsync();
            if (PackageTripDestination is null)
            {
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"Not Found PackageTrip  With Id : {AddDto.PackageTripId} Related With Destination with Id {AddDto.DestinationId}");

            }

            var requestAcivityIds = AddDto.ActivitiesDtos.Select(d => d.ActivityId)
                                                          .Distinct()
                                                          .ToList();

            if (requestAcivityIds.Count() != AddDto.ActivitiesDtos.Count())
            {
                return Result<GetPackageTripDestinationActivitiesDto>.BadRequest($"Duplicate Acitvites Ids found in the request. ");
            }

            var existActivities = await _activityRepositoryAsync.GetTableNoTracking()
                                                               .Where(a => requestAcivityIds.Contains(a.Id))
                                                               .ToListAsync();

            if (requestAcivityIds.Count() != existActivities.Count())
            {
                var notFoundActivitesIds = requestAcivityIds.Except(existActivities.Select(d => d.Id));
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"One or More from Activities Not found ,Missing Activity Ids : {string.Join(',', notFoundActivitesIds)} ");
            }

            // التحقق من أسعار الأنشطة
            foreach (var activityDto in AddDto.ActivitiesDtos)
            {
                var activity = existActivities.FirstOrDefault(a => a.Id == activityDto.ActivityId);
                if (activityDto.Price < activity!.Price)
                {
                    return Result<GetPackageTripDestinationActivitiesDto>.BadRequest($"Price for Activity With name {activity.Name} ({activityDto.Price}) is less than the allowed price ({activity?.Price ?? 0}).");
                }
            }
            var destinationActivities = await _destinationActivityRepositoryAsync.GetTableNoTracking()
                                                                       .Where(da => da.DestinationId == PackageTripDestination.DestinationId
                                                                                  && existActivities.Select(ea => ea.Id).Contains(da.ActivityId))
                                                                       .Include(d => d.Activity)
                                                                       .ToListAsync();

            if (destinationActivities.Count() != existActivities.Count())
            {
                var notFoundDestinationActivites = existActivities.Select(a => a.Id)
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
                        //Description = activity.Description,
                        //OrderActivity = activity.OrderActivity,
                        //Duration = activity.Duration,
                        //StartTime = activity.StartTime,
                        //EndTime = activity.EndTime,
                        ActivityId = activity.ActivityId,
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
                    ActivitiesDtos = destinationActivities.Select(da => new GetPackageTripDestinationActivityDto
                    {
                        //Description = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.Description,
                         Name =da.Activity.Name,
                        //Duration = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.Duration,
                        //StartTime = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.StartTime,
                        //EndTime = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.EndTime,
                        //OrderActivity = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.OrderActivity,
                        ActivityId = da.Activity.Id,
                        Price = da.Activity.Price
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

            var PackageTripDate = await _pacakgeTripDateRepositoryAsync.GetTableNoTracking()
                                                                .Where(t => t.PackageTripId == PackageTrip.Id)
                                                                .Take(2)
                                                                .ToListAsync();
                                                       
            if (PackageTripDate.Any())
            {
                if (PackageTripDate.Count() != 1 || PackageTripDate[0].Status != Data.Enums.PackageTripDateStatus.Draft)
                {
                    return Result.BadRequest($"You Cann't Update PackageTrip With Id : {PackageTrip.Id} Because you have already Published it Before");                  
                }
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


           
            return await base.UpdateAsync(PackageTripDestinationActivity.Id, UpdateDto);
        }

        public async Task<Result> DeletePackageTripDestinationActivity(int PackageTripId , int DestinationId,  int ActivityId)
        {
            var PackageTrip = await _packageTripRepoAsync.GetByIdAsync(PackageTripId);
            if (PackageTrip is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id : {PackageTripId}");
            }
            var Destination = await _destinationRepoAsync.GetByIdAsync(DestinationId);
            if (Destination is null)
            {
                return Result.NotFound($"Not Found Destination With Id : {DestinationId}");
            }
            var Activity = await _activityRepositoryAsync.GetByIdAsync(ActivityId);
            if (Activity is null)
            {
                return Result.NotFound($"Not Found Activity With Id : {Activity}");
            }

            var PackageTripDate = await _pacakgeTripDateRepositoryAsync.GetTableNoTracking()
                                                    .Where(t => t.PackageTripId == PackageTrip.Id)
                                                    .Take(2)
                                                    .ToListAsync();

            if (PackageTripDate.Any())
            {
                if (PackageTripDate.Count() != 1 || PackageTripDate[0].Status != Data.Enums.PackageTripDateStatus.Draft)
                {
                    return Result.BadRequest($"You Cann't  Delete any Activity For PackageTrip With Id : {PackageTrip.Id} Because you have already Published it Before");
                }
            }

            var PackageTripDestinationActivity = await _packageTripDestinationActivityRepoAsync.GetTableNoTracking()
                                                                                               .Include(x => x.PackageTripDestination)
                                                                                               .FirstOrDefaultAsync(x => x.PackageTripDestination.PackageTripId == PackageTripId
                                                                                                                      && x.PackageTripDestination.DestinationId == DestinationId
                                                                                                                      && x.ActivityId == ActivityId);
                                                                                               
                                                                                               
            if (PackageTripDestinationActivity is null)
            {
                return Result.NotFound($"Not Found PackageTrip With Id : {PackageTripId} Related With Destination With id : {DestinationId} and Activity With Id : {ActivityId}");

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
                return Result<GetPackageTripDestinationActivityByIdDto>.NotFound($"Not Found PackageTrip With Id : {PackageTripId} Retated With Destination with Id {DestinationId}");

            }
            var PackageTripDestinationActivity = await _packageTripDestinationActivityRepoAsync.GetTableNoTracking()
                                                                                              .FirstOrDefaultAsync(p => p.PackageTripDestinationId == PackageTripDestination.Id
                                                                                                                    && p.ActivityId == ActivityId);
            if (PackageTripDestinationActivity is null)
            {
                return Result<GetPackageTripDestinationActivityByIdDto>.NotFound($"Not Found PackageTrip with id {PackageTripId} Retated With Destination {DestinationId} Activity With Id : {ActivityId}");

            }
            var resultDto = new GetPackageTripDestinationActivityByIdDto
            {
                Id = PackageTripDestinationActivity.Id,
                ActivityId = PackageTripDestinationActivity.ActivityId,
                DestinationId = PackageTripDestination.DestinationId,
                PackageTripId = PackageTripDestination.PackageTripId,
                Price = PackageTripDestinationActivity.Price,
                Name = Activity.Name,
                //Description = PackageTripDestinationActivity.Description,
                //Duration = PackageTripDestinationActivity.Duration,
                //EndTime = PackageTripDestinationActivity.EndTime,
                //StartTime = PackageTripDestinationActivity.StartTime,
                //OrderActivity = PackageTripDestinationActivity.OrderActivity,

            };
            return Result<GetPackageTripDestinationActivityByIdDto>.Success(resultDto);
        }
    }
}
