using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.ActivityPhobia.Commands;
using TripAgency.Service.Feature.ActivityPhobia.Queries;

namespace TripAgency.Service.Abstracts
{
    public interface IUserPhobiaService
    {
        Task<Result> AddUserPhobias(AddUserPhobiasDto addUserPhobiasDto);
        Task<Result<GetUserPhobiasDto>> GetUserPhobiasAsync();
        Task<Result> DeleteUserPhobia(int PhobiaId);
    }
}
