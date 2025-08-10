using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public partial class ActivityPhobiasConfigurations
    {
        public class BookingPassengerConfigurations : IEntityTypeConfiguration<BookingPassenger>
        {
            public void Configure(EntityTypeBuilder<BookingPassenger> builder)
            {
                builder.HasKey(x => x.Id);

                builder.Property(x => x.FullName)
                       .HasColumnType("nvarchar")
                       .HasMaxLength(100)
                       .IsRequired();

                builder.Property(x => x.PhoneNumber)
                      .HasColumnType("nvarchar")
                      .HasMaxLength(50)
                      .IsRequired(false);

                builder.Property(x => x.Age)
                       .IsRequired();

                builder.HasOne(x => x.BookingTrip)
                       .WithMany(a => a.BookingPassengers)
                       .HasForeignKey(a => a.BookingTripId)
                       .OnDelete(DeleteBehavior.Restrict);

               
                builder.ToTable("BookingPassengers");

            }

        } 

}
