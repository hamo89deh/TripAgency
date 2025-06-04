using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class PackageTripTypeConfiguration : IEntityTypeConfiguration<PackageTripType>
    {
        public void Configure(EntityTypeBuilder<PackageTripType> builder)
        {
            builder.HasKey(h => h.Id);

            builder.ToTable("PackageTripTypes");

            builder.HasOne(c => c.PackageTrip)
                   .WithMany(h => h.PackageTripTypes)
                   .HasForeignKey(h => h.PackageTripId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.TypeTrip)
                 .WithMany(h => h.PackageTripTypes)
                 .HasForeignKey(h => h.TypeTripId)
                 .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
