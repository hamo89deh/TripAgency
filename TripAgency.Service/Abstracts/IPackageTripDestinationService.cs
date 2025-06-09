using TripAgency.Data.Entities;
using TripAgency.Service.Feature.PackageTripDestination.Commands;
using TripAgency.Service.Feature.PackageTripDestination.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IPackageTripDestinationService : IWriteService<PackageTripDestination , AddPackageTripDestinationDto , UpdatePackageTripDestinationDto , GetPackageTripDestinationByIdDto >
    {

    }
}
