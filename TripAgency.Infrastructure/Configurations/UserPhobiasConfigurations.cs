using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class UserPhobiasConfigurations : IEntityTypeConfiguration<UserPhobias>
    {
        public void Configure(EntityTypeBuilder<UserPhobias> builder)
        {
            builder.HasKey(x => new
            {
                x.UserId,
                x.PhobiaId
            });

            builder.HasOne(ap => ap.User)
                 .WithMany(a => a.UserPhobias)
                 .HasForeignKey(a => a.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ap => ap.Phobia)
                  .WithMany(a => a.UserPhobias)
                  .HasForeignKey(a => a.PhobiaId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("UserPhobias");

        }

    }

}
