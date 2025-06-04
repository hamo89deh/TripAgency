using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class PackageTripDestinationConfigurations : IEntityTypeConfiguration<PackageTripDestination>
    {
        public void Configure(EntityTypeBuilder<PackageTripDestination> builder)
        {
            builder.HasKey(x => x.Id);

            
            builder.ToTable("PackageTripDestinations");

            builder.HasOne(x=>x.PackageTrip)
                   .WithMany(x=>x.PackageTripDestinations)
                   .HasForeignKey(x=>x.PackageTripId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Destination)
                  .WithMany(x => x.PackageTripDestinations)
                  .HasForeignKey(x => x.DestinationId)
                  .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
