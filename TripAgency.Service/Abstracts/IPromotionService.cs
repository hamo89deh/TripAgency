using TripAgency.Data.Entities;
using TripAgency.Service.Generic;
using TripAgency.Service.Feature.PromotionDto;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.Trip.Queries;


namespace TripAgency.Service.Abstracts
{
    public interface IPromotionService :IReadService<Promotion , GetPromotionByIdDto, GetPromotionsDto>,
                                        IUpdateService<Promotion, UpdatePromotionDto>,
                                        IAddService<Promotion, AddPromotionDto, GetPromotionByIdDto>,
                                        IDeleteService<Promotion>
    {
        Task<Result<IEnumerable<GetPromotionsDto>>> GetPromotionsByPackageTripIdAsync(int packageTripId);
        Task<Result<GetPromotionByIdDto>> GetValidPromotionAsync(int packageTripId);
    }
}
