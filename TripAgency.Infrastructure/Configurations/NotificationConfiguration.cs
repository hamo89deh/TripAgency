using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(x => x.CreatedAt)
                    .HasColumnType("datetime2")
                    .IsRequired(true);

            builder.Property(x => x.Title)
                     .HasColumnType("nvarchar")
                     .HasMaxLength(50)
                     .IsRequired(true);

            builder.Property(x => x.Message)
                     .HasColumnType("nvarchar")
                     .HasMaxLength(250)
                     .IsRequired(true);

            builder.ToTable("Notifications");
        }
    }
}
