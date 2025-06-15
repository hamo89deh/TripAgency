using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
        public class PaymentConfigurations : IEntityTypeConfiguration<Payment>
        {
            public void Configure(EntityTypeBuilder<Payment> builder)
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.PaymentDate)
                       .HasColumnType("datetime2")
                       .IsRequired(true);

                builder.Property(x => x.PaymentStatus)
                        .HasConversion<int>()
                       .HasComment("Represents Booking status: 0 = Pending, 1 = Completed, 2 = Cancelled")
                       .IsRequired(true);

                builder.Property(x => x.Amount)
                       .HasPrecision(18, 2)
                       .IsRequired(true);

                builder.ToTable("Payments");

                builder.HasOne(x => x.BookingTrip)
                       .WithOne(x => x.Payment)
                       .HasForeignKey<Payment>(x => x.BookingTripId)
                       .OnDelete(DeleteBehavior.Restrict);



            }
        }
}
