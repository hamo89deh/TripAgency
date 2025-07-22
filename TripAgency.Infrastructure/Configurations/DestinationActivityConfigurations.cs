using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class DestinationActivityConfigurations : IEntityTypeConfiguration<DestinationActivity>
    {
        public void Configure(EntityTypeBuilder<DestinationActivity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x=>x.Destination)
                   .WithMany(x => x.DestinationActivities)
                   .HasForeignKey(x=>x.DestinationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Activity)
                    .WithMany(x => x.DestinationActivities)
                    .HasForeignKey(x => x.ActivityId)
                    .OnDelete(DeleteBehavior.Restrict);


            builder.ToTable("DestinationActivities");

        }

    }
}
