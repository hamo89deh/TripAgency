using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class TripDestinationConfigurations : IEntityTypeConfiguration<TripDestination>
    {
        public void Configure(EntityTypeBuilder<TripDestination> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Destination)
                   .WithMany(x => x.TripDestinations)
                   .HasForeignKey(x => x.DestinationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Trip)
                    .WithMany(x => x.TripDestinations)
                    .HasForeignKey(x => x.TripId)
                    .OnDelete(DeleteBehavior.Restrict);


            builder.ToTable("TripDestinations");

        }

    }
}
