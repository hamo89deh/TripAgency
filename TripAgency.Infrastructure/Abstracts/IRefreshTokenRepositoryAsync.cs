using TripAgency.Data.Entities.Identity;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Abstracts
{
    public interface IRefreshTokenRepositoryAsync : IGenericRepositoryAsync<UserRefreshToken>
    {
      
    }
}
