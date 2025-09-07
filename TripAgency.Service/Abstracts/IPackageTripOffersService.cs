using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.PromotionDto;

namespace TripAgency.Service.Abstracts
{
    public interface IPackageTripOffersService
    {
        Task<Result> DeletePackageTripOffer(int PackageTirpId, int OfferId);
        Task<Result> AddPackageTripOffer(int PackageTirpId, int OfferId);
        Task<Result> CancelAppliedOfferAsync(int packageTripId);
        Task<Result<IEnumerable<GetPackageTripOffersDto>>> GetOffersByPackageTripIdAsync(int packageTripId);
        Task<Result<GetOfferByIdDto>> GetValidOfferAsync(int packageTripId);
        Task<Result> ReapplyOfferAsync(int packageTripId, int offerId);
    }
}
