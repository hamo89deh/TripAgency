using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class OfferConfigurations : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            // تحديد المفتاح الأساسي
            builder.HasKey(x => x.Id);


            builder.Property(x => x.DiscountPercentage)
                   .HasPrecision(5, 2) // لتخزين نسب مثل 10.50
                   .IsRequired();

            builder.Property(x => x.Name)
                   .HasColumnType("varchar")
                   .HasMaxLength(100)
                   .IsRequired();
            builder.Property(x => x.StartDate)
                   .HasColumnType("dateTime2")
                   .IsRequired();

            builder.Property(x => x.EndDate)
                   .HasColumnType("dateTime2")
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(x => x.UpdatedAt)
                   .HasColumnType("datetime2")
                   .IsRequired();

            // تحديد اسم الجدول
            builder.ToTable("Offers");

        }
    }
}
