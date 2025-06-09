using TripAgency.Data.Entities;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Commands;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IPackageTripDestinationActivityService : IWriteService<PackageTripDestinationActivity, AddPackageTripDestinationActivityDto, UpdatePackageTripDestinationActivityDto, GetPackageTripDestinationActivitiesDto>
    {

    }
}
