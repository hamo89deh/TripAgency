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
                   .WithMany(h => h.PackageTrips)
                   .HasForeignKey(h => h.TripId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
