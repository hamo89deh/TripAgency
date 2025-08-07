using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities.Identity;

namespace TripAgency.Infrastructure.Configurations
{
    public class RefreshTokenConfigurations : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RefreshToken)
                   .HasColumnType("nvarchar")
                   .IsRequired(true);

            builder.Property(x => x.Token)
                    .HasColumnType("nvarchar")
                   .IsRequired(false);

            builder.HasOne(r=>r.User)
                   .WithMany(r=>r.UserRefreshTokens)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("UserRefreshTokens");
                    
        }
    }
}
