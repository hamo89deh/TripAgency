using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public partial class ActivityPhobiasConfigurations : IEntityTypeConfiguration<ActivityPhobias>
    {
        public void Configure(EntityTypeBuilder<ActivityPhobias> builder)
        {
            builder.HasKey(x => new
            {
                x.ActivityId,
                x.PhobiaId
            });
           
            builder.HasOne(ap => ap.Activity)
                   .WithMany(a => a.ActivityPhobias)
                   .HasForeignKey(a => a.ActivityId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ap => ap.Phobia)
                  .WithMany(a => a.ActivityPhobias)
                  .HasForeignKey(a => a.PhobiaId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("ActivityPhobias");

        }

    }

}
