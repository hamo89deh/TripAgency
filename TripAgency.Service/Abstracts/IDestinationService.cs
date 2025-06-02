using TripAgency.Data.Entities;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.City.Command;
using TripAgency.Service.Feature.City.Queries;
using TripAgency.Service.Feature.Destination.Commands;
using TripAgency.Service.Feature.Destination.Queries;

namespace TripAgency.Service.Abstracts
{
    public interface IDestinationService 
    {
        Task<Result<IEnumerable<GetDestinationsByCityNameDto>>> GetDestinationsByCityName(string cityName);

        Task<Result<IEnumerable<GetDestinationsDto>>> GetDestinationsAsync();
        Task<Result<GetDestinationByIdDto>> GetDestinationByIdAsync(int id);
        Task<Result> CreateDestinationAsync(AddDestinationDto addDestinationDto);
        Task<Result> UpdateDestinationAsync(EditDestinationDto updateDestinationDto);
        Task<Result> DeleteDestinationAsync(int id);
    }
}
