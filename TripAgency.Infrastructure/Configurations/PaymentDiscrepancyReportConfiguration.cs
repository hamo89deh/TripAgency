using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class PaymentDiscrepancyReportConfiguration : IEntityTypeConfiguration<PaymentDiscrepancyReport>
    {
        public void Configure(EntityTypeBuilder<PaymentDiscrepancyReport> builder)
        {
            // المفتاح الأساسي
            builder.HasKey(r => r.Id);

            // الخصائص المطلوبة وأطوالها
            builder.Property(r => r.ReportedTransactionRef).IsRequired().HasMaxLength(200);
            builder.Property(r => r.ReportedPaymentDateTime).IsRequired();
            builder.Property(r => r.ReportedPaidAmount).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(r => r.ReportDate).IsRequired();
            builder.Property(r => r.Status).IsRequired();
            builder.Property(r => r.CustomerNotes).HasMaxLength(500); // اختياري

            // الأعمدة الخاصة بالتدقيق
            builder.Property(r => r.CreatedAt).IsRequired().HasDefaultValueSql("GETDATE()"); // تعيين قيمة افتراضية لتاريخ الإنشاء
            builder.Property(r => r.UpdatedAt).IsRequired().HasDefaultValueSql("GETDATE()"); // تعيين قيمة افتراضية لتاريخ التحديث

            builder.HasOne(r => r.User) // المستخدم الذي قدم البلاغ
                   .WithMany()
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.ReviewedByUser) // المسؤول الذي راجع البلاغ
                   .WithMany()
                   .HasForeignKey(r => r.ReviewedByUserId)
                   .IsRequired(false) // يمكن أن يكون null قبل المراجعة
                   .OnDelete(DeleteBehavior.SetNull); // عند حذف المستخدم المسؤول، يصبح هذا الحقل null


            // اسم الجدول في قاعدة البيانات
            builder.ToTable("PaymentDiscrepancyReports");
        }
    }
}
