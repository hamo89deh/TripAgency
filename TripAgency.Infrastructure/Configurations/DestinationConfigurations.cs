using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class DestinationConfigurations : IEntityTypeConfiguration<Destination>
    {
        public void Configure(EntityTypeBuilder<Destination> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                  .HasColumnType("nvarchar")
                  .HasMaxLength(50)
                  .IsRequired(true);

            builder.Property(x=>x.Description)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(250)
                   .IsRequired(true);

            builder.Property(x => x.Location)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(250)
                   .IsRequired(true);

            builder.HasOne(x => x.City)
                   .WithMany(x => x.Destinations)
                   .HasForeignKey(x => x.CityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(h => h.ImageUrl)
             .IsRequired()
             .HasMaxLength(300);

            builder.ToTable("Destinations");


        }
    }
}
