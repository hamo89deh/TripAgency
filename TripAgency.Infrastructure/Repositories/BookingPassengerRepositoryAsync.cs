using TripAgency.Data.Entities;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure.InfrastructureBases;

namespace TripAgency.Infrastructure.Repositories
{
    public class BookingPassengerRepositoryAsync : GenericRepositoryAsync<BookingPassenger>, IBookingPassengerRepositoryAsync
    {
        public BookingPassengerRepositoryAsync(TripAgencyDbContext dbContext) : base(dbContext)
        {

        }
    }
}
