using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.Description)
                .IsRequired()
                .HasMaxLength(400);

            builder.Property(h => h.ImageUrl)
              .IsRequired()
              .HasMaxLength(300);

            builder.ToTable("Trips");          

        }
    }
}
