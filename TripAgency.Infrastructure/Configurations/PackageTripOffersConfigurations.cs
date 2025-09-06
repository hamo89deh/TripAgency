using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class PackageTripOffersConfigurations : IEntityTypeConfiguration<PackageTripOffers>
    {

        public void Configure(EntityTypeBuilder<PackageTripOffers> builder)
        {
            // تحديد المفتاح الأساسي
            builder.HasKey(x => x.Id);

            // تحديد اسم الجدول
            builder.ToTable("PackageTripOffers");

            // تحديد العلاقة مع PackageTrip
            builder.HasOne(x => x.PackageTrip)
                   .WithMany(x => x.PackageTripOffers)
                   .HasForeignKey(x => x.PackageTripId)
                   .OnDelete(DeleteBehavior.Restrict);

            // تحديد العلاقة مع PackageTrip
            builder.HasOne(x => x.Offer)
                   .WithMany(x => x.PackageTripOffers)
                   .HasForeignKey(x => x.OfferId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
