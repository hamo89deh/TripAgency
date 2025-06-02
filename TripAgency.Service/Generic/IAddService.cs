using TripAgency.Data.Result.TripAgency.Core.Results;

namespace TripAgency.Service.Generic
{
    public interface IAddService<T, AddDto,GetByIdDto>
    {
        Task<Result<GetByIdDto>> CreateAsync(AddDto addDto);
    }


}
