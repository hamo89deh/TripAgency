using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class TypeTripConfiguration : IEntityTypeConfiguration<TypeTrip>
    {
        public void Configure(EntityTypeBuilder<TypeTrip> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.ToTable("TypeTrips");
        }
    }
}
