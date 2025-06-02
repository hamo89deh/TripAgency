using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.City.Command;
using TripAgency.Service.Feature.City.Queries;
using TripAgency.Service.Feature.Destination.Commands;
using TripAgency.Service.Feature.Destination.Queries;
using TripAgency.Service.Generic;

namespace TripAgency.Service.Abstracts
{
    public interface IDestinationService : IReadAndDeleteService<Destination, GetDestinationByIdDto , GetDestinationsDto>
    {
        Task<Result<IEnumerable<GetDestinationsByCityNameDto>>> GetDestinationsByCityName(string cityName);

        Task<Result> CreateDestinationAsync(AddDestinationDto addDestinationDto);
        Task<Result> UpdateDestinationAsync(EditDestinationDto updateDestinationDto);
    }
}
