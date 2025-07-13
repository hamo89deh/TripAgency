using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class BookingTripConfigurations : IEntityTypeConfiguration<BookingTrip>
    {
        public void Configure(EntityTypeBuilder<BookingTrip> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Notes)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(50)
                   .IsRequired(true);

            builder.Property(x => x.BookingDate)
                   .HasColumnType("datetime2")
                   .IsRequired(true);

            builder.Property(x => x.PassengerCount)
                  .IsRequired(true);

            builder.Property(x => x.BookingStatus)
                    .HasConversion<int>()
                   .HasComment("Represents Booking status: 0 = Pending, 1 = Completed, 2 = Cancelled")
                   .IsRequired(true);

            builder.Property(x => x.ActualPrice)
                   .HasPrecision(18, 2)
                   .IsRequired(true);

            builder.ToTable("BookingTrips");
            
            builder.HasOne(x=>x.PackageTripDate)
                   .WithMany(x=>x.BookingTrips)
                   .HasForeignKey(x=>x.PackageTripDateId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                 .WithMany(x => x.BookingTrips)
                 .HasForeignKey(x => x.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
