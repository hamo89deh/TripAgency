using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.City.Command;

namespace TripAgency.Service.Generic
{
    public interface IReadAndDeleteService<T,GetByIdDto , GetALlDto> 
    {
        Task<Result<IEnumerable<GetALlDto>>> GetAllAsync();
        Task<Result<GetByIdDto>> GetByIdAsync(int id);
        Task<Result> DeleteAsync(int id);

    }


}
