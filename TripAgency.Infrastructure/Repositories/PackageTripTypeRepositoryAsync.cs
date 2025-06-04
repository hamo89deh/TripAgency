using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class PackageTripTypeRepositoryAsync : GenericRepositoryAsync<PackageTripType>, IPackageTripTypeRepositoryAsync
    {
        public PackageTripTypeRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {
        }
    }

}
