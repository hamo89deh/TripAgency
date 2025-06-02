using TripAgency.Data.Result.TripAgency.Core.Results;

namespace TripAgency.Service.Generic
{
    public interface IDeleteService<T> 
    {
        Task<Result> DeleteAsync(int id);

    }

}
