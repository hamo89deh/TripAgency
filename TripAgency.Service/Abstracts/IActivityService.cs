using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Generic;
using TripAgency.Service.Feature.Activity.Queries;
using TripAgency.Service.Feature.Activity.Commands;
using TripAgency.Data.Entities.Identity;


namespace TripAgency.Service.Abstracts
{
    public interface IActivityService : IReadService<Activity, GetActivityByIdDto, GetActivitiesDto>,
                                   IUpdateService<Activity, UpdateActivityDto>,
                                   IAddService<Activity, AddActivityDto, GetActivityByIdDto>,
                                   IDeleteService<Activity>
    {
        Task<Result<GetActivityByIdDto>> GetActivityByNameAsync(string name);


    }
}
