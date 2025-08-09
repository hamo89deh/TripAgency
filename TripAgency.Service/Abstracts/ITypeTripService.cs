using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Generic;
using TripAgency.Service.Feature.Trip.Queries;
using TripAgency.Service.Feature.TypeTrip_Entity.Queries;
using TripAgency.Service.Feature.TypeTrip_Entity.Commands;


namespace TripAgency.Service.Abstracts
{
    public interface ITripTypeService : IReadService<TypeTrip, GetTypeTripByIdDto, GetTripTypesDto>,
                                   IUpdateService<TypeTrip, UpdateTypeTripDto>,
                                   IAddService<TypeTrip, AddTypeTripDto, GetTypeTripByIdDto>,
                                   IDeleteService<TypeTrip>
    {
        Task<Result<GetTypeTripByIdDto>> GetTypeTripByNameAsync(string name);


    }
}
