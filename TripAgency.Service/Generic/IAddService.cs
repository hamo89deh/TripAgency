using TripAgency.Data.Result.TripAgency.Core.Results;

namespace TripAgency.Service.Generic
{
    public interface IAddService<T, AddDto>
    {
        Task<Result> CreateAsync(AddDto addCityDto);
    }


}
