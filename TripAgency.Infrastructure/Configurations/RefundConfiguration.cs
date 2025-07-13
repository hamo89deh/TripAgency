using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class RefundConfiguration : IEntityTypeConfiguration<Refund>
    {
        public void Configure(EntityTypeBuilder<Refund> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(x => x.CreatedAt)
                    .HasColumnType("datetime2")
                    .IsRequired(true);

            builder.Property(x => x.Status)
                    .HasConversion<int>()
                   .HasComment(" status: 0 = Pending, 1 = Completed, 2 = Failed")
                   .IsRequired(true);

            builder.Property(x => x.Amount)
                   .HasPrecision(18, 2)
                   .IsRequired(true);

            builder.HasOne(x => x.BookingTrip)
                   .WithOne(x => x.Refund)
                   .HasForeignKey<Refund>(x => x.BookingTripId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Refunds");
        }
    }
}
