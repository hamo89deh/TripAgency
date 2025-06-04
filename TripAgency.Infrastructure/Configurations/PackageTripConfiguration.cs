using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class PackageTripConfiguration : IEntityTypeConfiguration<PackageTrip>
    {
        public void Configure(EntityTypeBuilder<PackageTrip> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.Description)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.MaxCapacity)
                .IsRequired();

            builder.Property(h => h.MinCapacity)
                .IsRequired();

            builder.Property(h => h.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(h => h.Duration)
                .IsRequired();

            builder.Property(h => h.CancellationPolicy)
               .IsRequired()
               .HasMaxLength(200);

            builder.ToTable("PackageTrips");

            builder.HasOne(c => c.Trip)
                   .WithMany(h => h.TripList)
                   .HasForeignKey(h => h.TripId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }

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
