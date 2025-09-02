using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.PackageTripDestination.Commands;
using TripAgency.Service.Feature.PackageTripDestination.Queries;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IPackageTripDestinationService : IWriteService<PackageTripDestination , AddPackageTripDestinationDto , UpdatePackageTripDestinationDto , GetPackageTripDestinationsDto>
    {
        Task<Result<GetPackageTripDestinationByIdDto>> GetPackageTripDestination(int packageTripId , int destinationId);
        Task<Result<GetPackageTripDestinationsDto>> GetPackageTripDestinations(int packageTripId );
        Task<Result<GetPackageTripDestinationActivitiesDto>> GetPackageTripDestinationActivities(int packageTripId , int destinationId );
        Task<Result<GetPackageTripDestinationsActivitiesDto>> GetPackageTripDestinationsActivities(int packageTripId );
        Task<Result> DeletePackageTripDestinationAsync(int PackageTripId, int DestinationId);
    }
}
