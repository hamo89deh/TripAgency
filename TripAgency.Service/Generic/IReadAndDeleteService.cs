using TripAgency.Data.Result.TripAgency.Core.Results;

namespace TripAgency.Service.Generic
{
    public interface IReadAndDeleteService<T,GetByIdDto , GetALlDto>
    {
        Task<Result<IEnumerable<GetALlDto>>> GetAllAsync();
        Task<Result<GetByIdDto>> GetByIdAsync(int id);
        Task<Result> DeleteAsync(int id);

    }


}
