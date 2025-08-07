using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Favorite;


namespace TripAgency.Service.Abstracts
{
    public interface IFavoritePackageTripService 
    {
        Task<Result<IEnumerable< GetFavoritePackageTripsDto>>> GetFavoritePackageTripsDto();
        Task<Result> AddFavoritePackageTripDto(int PackageTripId);
        Task<Result> DeleteFavoritePackageTripDto(int PackageTripId);
    }
}
