using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Feature.Activity.Queries;
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
        public IPackageTripDestinationActivityRepositoryAsync _packageTripDestinationActivityRepoAsync { get; }
        public PackageTripDestinationActivityService(IPackageTripDestinationRepositoryAsync packageTripDestinationRepoAsync,
                                                     IPackageTripDestinationActivityRepositoryAsync packageTripDestinationActivityRepoAsync,
                                                     IMapper mapper,
                                                     IActivityRepositoryAsync activityRepositoryAsync,
                                                     IDestinationActivityRepositoryAsync destinationActivityRepositoryAsync,
                                                     ITripDateRepositoryAsync tripDateRepositoryAsync) : base(packageTripDestinationActivityRepoAsync, mapper)
        {
            _packageTripDestinationRepoAsync = packageTripDestinationRepoAsync;
            _packageTripDestinationActivityRepoAsync = packageTripDestinationActivityRepoAsync;
            _activityRepositoryAsync = activityRepositoryAsync;
            _destinationActivityRepositoryAsync = destinationActivityRepositoryAsync;
            _tripDateRepositoryAsync = tripDateRepositoryAsync;
        }
        public override async Task<Result<GetPackageTripDestinationActivitiesDto>> CreateAsync(AddPackageTripDestinationActivityDto AddDto)
        {
            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetByIdAsync(AddDto.PackageTripDestinationId);
           
            if (PackageTripDestination is null)
            {
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"Not Found PackageTripDestination With Id : {AddDto.PackageTripDestinationId}");          
           
            }
            
            var requestAcivityIds =  AddDto.ActivitiesDtos.Select(d => d.ActivityId)  
                                                          .Distinct()          
                                                          .ToList();

            if(requestAcivityIds.Count() != AddDto.ActivitiesDtos.Count()) 
            {
                return Result<GetPackageTripDestinationActivitiesDto>.BadRequest($"Duplicate Acitvites Ids found in the request. ");
            }
         
            var existActivites = await _activityRepositoryAsync.GetTableNoTracking()
                                                               .Where(a=>requestAcivityIds.Contains(a.Id))
                                                               .ToListAsync();
            
            if(requestAcivityIds.Count() != existActivites.Count())
            {
                var notFoundActivitesIds = requestAcivityIds.Except(existActivites.Select(d=>d.Id));
                return Result<GetPackageTripDestinationActivitiesDto>.NotFound($"One or More from Activities Not found ,Missing Activity Ids : {string.Join(',', notFoundActivitesIds)} ");
            }

            var destinationActivities = await _destinationActivityRepositoryAsync.GetTableNoTracking()
                                                                       .Where(da=>da.DestinationId == PackageTripDestination.DestinationId 
                                                                                  && existActivites.Select(ea=>ea.Id).Contains(da.ActivityId))
                                                                       .Include(d=>d.Activity)
                                                                       .ToListAsync();

            if(destinationActivities.Count() != existActivites.Count())
            {
                var notFoundDestinationActivites = existActivites.Select(a=>a.Id)
                                                                 .Except(destinationActivities.Select(d => d.Id));

                return Result<GetPackageTripDestinationActivitiesDto>.BadRequest($"The Destination With Id : {PackageTripDestination.DestinationId} Not Contain Activity with id {string.Join(',' , notFoundDestinationActivites)}");
            }

            var PackageTripDestinationActivitiesToAdd = new List<PackageTripDestinationActivity>();

            foreach (var activity in AddDto.ActivitiesDtos)
            {
                // Check if the trip already has this Activity to avoid duplicates
                if (!PackageTripDestination.PackageTripDestinationActivities.Any(td => td.ActivityId == activity.ActivityId))
                {
                    PackageTripDestinationActivitiesToAdd.Add(new PackageTripDestinationActivity
                    {
                        ActivityId =activity.ActivityId,
                        Description = activity.Description ,
                        OrderActivity= activity.OrderActivity,
                        Duration = activity.Duration ,
                        StartTime = activity.StartTime ,
                        EndTime = activity.EndTime ,
                        Price = activity.Price ,
                        PackageTripDestinationId= PackageTripDestination.Id
                        
                    });
                }
            }
          
            if (PackageTripDestinationActivitiesToAdd.Any())
            {
                await _packageTripDestinationActivityRepoAsync.AddRangeAsync(PackageTripDestinationActivitiesToAdd);
            }
         

            var resultDto = new GetPackageTripDestinationActivitiesDto()
            {
                PackageTripDestinationId = PackageTripDestination.Id,
                ActivitiesDtos = destinationActivities.Select(da=> new PackageTripDestinationActivitiesDto
                {
                    ActivityId = da.Activity.Id,
                    Description = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x=>x.ActivityId==da.ActivityId)!.Description,
                   // Name =da.Activity.Name,
                    Price =da.Activity.Price ,
                    Duration = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.Duration ,
                    StartTime = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.StartTime ,
                    EndTime= PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.EndTime ,
                    OrderActivity  = PackageTripDestinationActivitiesToAdd.FirstOrDefault(x => x.ActivityId == da.ActivityId)!.OrderActivity
                })
            };
            
            
            return Result<GetPackageTripDestinationActivitiesDto>.Success(resultDto);
        }
        public override async Task<Result> UpdateAsync(int id, UpdatePackageTripDestinationActivityDto UpdateDto)
        {
            var PackageTripDestinationActivity = await _packageTripDestinationActivityRepoAsync.GetByIdAsync(id);
            if (PackageTripDestinationActivity is null)
            {
                return Result.NotFound($"Not Found PackageTripDestinationActivity With Id : {id}");

            }

            var PackageTripDestination = await _packageTripDestinationRepoAsync.GetByIdAsync(UpdateDto.PackageTripDestinationId);
            if (PackageTripDestination is null)
            {
                return Result.NotFound($"Not Found PackageTripDestination With Id : {UpdateDto.PackageTripDestinationId}");
            }

            var TripDate = await _tripDateRepositoryAsync.GetTableNoTracking()
                                                          .Where(t => t.PackageTripId == PackageTripDestination.PackageTripId)
                                                          .LastOrDefaultAsync();
            if(TripDate is not null)
            {
                return Result.BadRequest($"You Cann't Update PackageTrip Activities Because you have TripDate with Id: {TripDate.Id}");

            }           
            return await base.UpdateAsync(id, UpdateDto);
        }
       
        public override async Task<Result> DeleteAsync(int id)
        {
            var PackageTripDestinationActivity = await _packageTripDestinationActivityRepoAsync.GetTableNoTracking()
                                                                                               .Where(x=>x.Id==id)
                                                                                               .Include(x=>x.PackageTripDestination)
                                                                                               .FirstOrDefaultAsync();
            if (PackageTripDestinationActivity is null)
            {
                return Result.NotFound($"Not Found PackageTripDestinationActivity With Id : {id}");

            }

            var TripDate = await _tripDateRepositoryAsync.GetTableNoTracking()
                                                          .Where(t => t.PackageTripId == PackageTripDestinationActivity.PackageTripDestination.PackageTripId)
                                                          .LastOrDefaultAsync();
            if (TripDate is not null)
            {
                return Result.BadRequest($"Cann't Delete PackageTripDestinationActivity Because you have TripDate :{TripDate.Id} Connected before with PackageTrip: {PackageTripDestinationActivity.PackageTripDestination.PackageTripId}");

            }
            await _packageTripDestinationActivityRepoAsync.DeleteAsync(PackageTripDestinationActivity);
            return Result.Success();
        }

    }
}
