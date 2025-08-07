using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities.Identity;

namespace TripAgency.Infrastructure.Configurations
{
    public class FavoritePackageTripConfigurations : IEntityTypeConfiguration<FavoritePackageTrip>
    {
        public void Configure(EntityTypeBuilder<FavoritePackageTrip> builder)
        {
            builder.HasKey(x => new
            {
                x.UserId,
                x.PackageTripId
            });

            builder.HasOne(f=>f.User)
                   .WithMany(u=>u.FavoritePackageTrips)
                   .HasForeignKey(f=>f.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(f => f.PackageTrip)
                    .WithMany(u => u.FavoritePackageTrips)
                    .HasForeignKey(f => f.PackageTripId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("FavoritePackageTrips");

        }

    }
}
