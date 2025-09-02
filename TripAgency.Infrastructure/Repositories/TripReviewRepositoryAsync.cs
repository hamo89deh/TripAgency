using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class TripReviewRepositoryAsync : GenericRepositoryAsync<TripReview>, ITripReviewRepositoryAsync
    {
        public TripReviewRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
