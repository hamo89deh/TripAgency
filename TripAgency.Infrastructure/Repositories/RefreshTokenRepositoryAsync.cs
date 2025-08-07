using TripAgency.Data.Entities.Identity;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class RefreshTokenRepositoryAsync : GenericRepositoryAsync<UserRefreshToken>, IRefreshTokenRepositoryAsync
    {
        public RefreshTokenRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }      
    }
}
