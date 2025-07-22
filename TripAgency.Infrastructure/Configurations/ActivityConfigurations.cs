using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Activity = TripAgency.Data.Entities.Activity;

namespace TripAgency.Infrastructure.Configurations
{
    public class ActivityConfigurations : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(50)
                   .IsRequired(true);

            builder.Property(x => x.Description)
                  .HasColumnType("nvarchar")
                  .HasMaxLength(250)
                  .IsRequired(true);

            builder.Property(x => x.Price)
                   .HasPrecision(18,2)
                   .IsRequired(true);

            builder.ToTable("Activities");

        }

    }
}
