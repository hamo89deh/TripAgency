using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class TripReviewConfigurations : IEntityTypeConfiguration<TripReview>
    {
        public void Configure(EntityTypeBuilder<TripReview> builder)
        {
            // تحديد المفتاح الأساسي
            builder.HasKey(x => x.Id);

            // تحديد خصائص الحقول
            builder.Property(x => x.PackageTripDateId)
                   .IsRequired();

            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.Rating)
                   .IsRequired();

            builder.Property(x => x.Comment)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(200) // حد أقصى للتعليق
                   .IsRequired(false); // التعليق اختياري

            builder.Property(x => x.CreatedAt)
                   .HasColumnType("datetime2")
                   .IsRequired();

            // تحديد اسم الجدول
            builder.ToTable("TripReviews");

            // تحديد العلاقات
            builder.HasOne(x => x.PackageTripDate)
                   .WithMany(x => x.TripReviews)
                   .HasForeignKey(x => x.PackageTripDateId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                   .WithMany(x => x.TripReviews)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            // إضافة فهرس فريد على UserId و PackageTripDateId
            builder.HasIndex(x => new { x.UserId, x.PackageTripDateId })
                   .IsUnique()
                   .HasDatabaseName("IX_TripReviews_UserId_PackageTripDateId_Unique");
        }
    }
}
