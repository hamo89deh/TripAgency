using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class UserPhobiasRepositoryAsync : GenericRepositoryAsync<UserPhobias>, IUserPhobiasRepositoryAsync
    {
        public UserPhobiasRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
