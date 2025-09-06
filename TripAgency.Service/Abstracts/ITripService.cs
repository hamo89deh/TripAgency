using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Generic;
using TripAgency.Service.Feature.Trip.Queries;
using TripAgency.Service.Feature.Trip.Commands;
using TripAgency.Service.Feature.PackageTripDestinationActivity.Queries;
using TripAgency.Data.Helping;


namespace TripAgency.Service.Abstracts
{
    public interface ITripService : IReadService<Trip, GetTripByIdDto, GetTripsDto>,
                                   IUpdateService<Trip, UpdateTripDto>,
                                   IAddService<Trip, AddTripDto, GetTripByIdDto>,
                                   IDeleteService<Trip>
    {
        Task<Result<GetTripByIdDto>> GetTripByNameAsync(string name);
        Task<Result<GetTripDestinationsDto>> AddTripDestinations(AddTripDestinationsDto addTripDestinationsDto);
        Task<Result<GetTripDestinationsDto>> GetTripDestinationsById(int TripId);
        Task<Result> DeleteTripDestination(int tripId, int destinationId);
        Task<Result<IEnumerable<GetTripsDto>>> GetTripsForUsersAsync();
        Task<Result<PaginatedResult<GetTripsDto>>> GetTripsPaginationForUsersAsync(string? search, int pageNumber, int pageSize );
        public Task<Result<GetPackageTripsForTripDto>> GetPackagesForTripAsync(int TripId);
        public Task<Result<PaginatedResult<PackageTripForTripDto>>> GetPackagesPaginationForTripAsync(int TripId, string? search, int pageNumber, int pageSize);

    }
}
