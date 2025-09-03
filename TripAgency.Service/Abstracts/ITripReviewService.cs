using TripAgency.Data.Entities;
using TripAgency.Service.Generic;
using TripAgency.Data.Result.TripAgency.Core.Results;
using TripAgency.Service.Feature.TripReview;


namespace TripAgency.Service.Abstracts
{
    public interface ITripReviewService: IReadService<TripReview, GetTripReviewByIdDto, GetTripReviewsDto>,
                                         IAddService<TripReview ,AddTripReviewDto, GetTripReviewByIdDto>,
                                         IDeleteService<TripReview>
    {
        Task<Result<IEnumerable<GetTripReviewsDto>>> GetReviewsByPackageTripDateIdAsync(int packageTripDateId);
        Task<Result<bool>> CanUserReviewAsync(int packageTripDateId);
        Task<int> CalculateAverageRatingAsync(int packageTripId);

    }
}
