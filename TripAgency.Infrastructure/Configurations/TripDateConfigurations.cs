using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class TripDateConfigurations : IEntityTypeConfiguration<TripDate>
    {
        public void Configure(EntityTypeBuilder<TripDate> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x=>x.StartTripDate)
                   .HasColumnType ("datetime2")
                   .IsRequired ();

            builder.Property(x => x.EndTripDate)
                   .HasColumnType("datetime2")
                   .IsRequired();

            builder.Property(td => td.StartBookingDate)
                   .IsRequired()
                   .HasColumnType("datetime2");

            builder.Property(td => td.EndBookingDate)
                   .IsRequired()
                   .HasColumnType("datetime2");

            builder.Property(td => td.AvailableSeats)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(td => td.Status)
                   .IsRequired()
                   .HasConversion<int>()
                   .HasComment("Represents trip status: 0 = Available, 1 = Completed, 2 = Cancelled, 3 = Planned");

            builder.Property(td => td.CreateDate)
                   .IsRequired()
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(td => td.IsAvailable)
                   .IsRequired()
                   .HasDefaultValue(true);

            // builder.HasCheckConstraint("CK_TripDate_TripDates", "[StartTripDate] < [EndTripDate]");


            builder.ToTable("TripDates");

            builder.HasOne(s=>s.PackageTrip)
                   .WithMany(d=>d.TripDates)
                   .HasForeignKey(d => d.PackageTripId)
                   .OnDelete(DeleteBehavior.Restrict);
        }

    }

}
