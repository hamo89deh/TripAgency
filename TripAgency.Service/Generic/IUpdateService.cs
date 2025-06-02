using TripAgency.Data.Result.TripAgency.Core.Results;

namespace TripAgency.Service.Generic
{
    public interface IUpdateService<T, UpdateDto>
    {
        Task<Result> UpdateAsync(UpdateDto updateCityDto);
    }


}
