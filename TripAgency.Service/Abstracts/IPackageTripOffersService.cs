using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.PromotionDto;

namespace TripAgency.Service.Abstracts
{
    public interface IPackageTripOffersService
    {
        Task<Result> DeletePackageTripOffer(int PackageTirpId, int OfferId);
        Task<Result> AddPackageTripOffer(int PackageTirpId, int OfferId);
    }
}
