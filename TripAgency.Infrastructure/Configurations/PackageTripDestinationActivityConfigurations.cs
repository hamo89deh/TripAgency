using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class PackageTripDestinationActivityConfigurations : IEntityTypeConfiguration<PackageTripDestinationActivity>
    {
        public void Configure(EntityTypeBuilder<PackageTripDestinationActivity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x=>x.Price)
                   .HasPrecision(18,2)
                   .IsRequired();

            builder.ToTable("PackageTripDestinationActivities");

            builder.HasOne(x => x.PackageTripDestination)
                   .WithMany(x => x.PackageTripDestinationActivities)
                   .HasForeignKey(x => x.PackageTripDestinationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Activity)
                  .WithMany(x => x.PackageTripDestinationActivities)
                  .HasForeignKey(x => x.ActivityId)
                  .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
