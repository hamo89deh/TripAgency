using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class PromotionConfigurations : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            // تحديد المفتاح الأساسي
            builder.HasKey(x => x.Id);

            // تحديد خصائص الحقول
            builder.Property(x => x.PackageTripId)
                   .IsRequired();

            builder.Property(x => x.DiscountPercentage)
                   .HasPrecision(5, 2) // لتخزين نسب مثل 10.50
                   .IsRequired();

            builder.Property(x => x.StartDate)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(x => x.EndDate)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(x => x.IsActive)
                   .IsRequired();

            builder.Property(x => x.IsDeleted)
                   .IsRequired().HasDefaultValue(false);

            builder.Property(x => x.CreatedAt)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(x => x.UpdatedAt)
                   .HasColumnType("datetime2")
                   .IsRequired();

            // تحديد اسم الجدول
            builder.ToTable("Promotions");

            // تحديد العلاقة مع PackageTrip
            builder.HasOne(x => x.PackageTrip)
                   .WithOne(x => x.Promotion)
                   .HasForeignKey<Promotion>(x => x.PackageTripId)
                   .OnDelete(DeleteBehavior.Restrict); // منع الحذف التلقائي
                                                       // إضافة فهرس فريد على PackageTripId لضمان One-to-One
            builder.HasIndex(x => x.PackageTripId)
                   .IsUnique()
                   .HasDatabaseName("IX_Promotions_PackageTripId_Unique");
        }
    }
}
