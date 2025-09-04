using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.City.Command;
using TripAgency.Service.Feature.City.Queries;
using TripAgency.Service.Feature.Destination.Commands;
using TripAgency.Service.Feature.Destination.Queries;
using TripAgency.Service.Feature.DestinationActivity.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IDestinationService : IReadService<Destination, GetDestinationByIdDto , GetDestinationsDto> ,
                                           IAddService<Destination ,AddDestinationDto ,GetDestinationByIdDto>,
                                           IUpdateService<Destination ,UpdateDestinationDto> , 
                                           IDeleteService<Destination>

                                          
    {
        Task<Result<IEnumerable<GetDestinationsByCityNameDto>>> GetDestinationsByCityName(string cityName);
        Task<Result> AddDestinationActivity(int DestinationId, int ActivityId);
        Task<Result> DeleteDestinationActivity(int DestinationId, int ActivityId);
        Task<Result<GetDestinationActivitiesByIdDto>> GetDestinationActivitiesByIdDto(int DestinationId);
        Task<Result<IEnumerable<GetDestinationsDetailsDto>>> GetDestinationsDetails();

       
    }
}
