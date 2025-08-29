using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Generic;
using TripAgency.Service.Feature.Trip.Queries;
using TripAgency.Service.Feature.Trip.Commands;


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
        Task<Result<GetTripByIdDto>> GetByIdAsync(int id);
    }
}
