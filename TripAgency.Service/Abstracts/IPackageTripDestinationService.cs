using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.PackageTripDestination.Commands;
using TripAgency.Service.Feature.PackageTripDestination.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IPackageTripDestinationService : IWriteService<PackageTripDestination , AddPackageTripDestinationDto , UpdatePackageTripDestinationDto , GetPackageTripDestinationByIdDto >
    {
        Task<Result<GetPackageTripDestinationByIdDto>> GetPackageTripDestination(int packageTripId , int destinationId);
    }
}
