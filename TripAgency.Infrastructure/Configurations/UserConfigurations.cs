using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities.Identity;

namespace TripAgency.Infrastructure.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Address)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.Property(x => x.FullName)
                    .HasColumnType("nvarchar")
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.Country)
                    .HasColumnType("nvarchar")
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.Property(x => x.Code)
                   .HasColumnType("nvarchar")
                  .HasMaxLength(25)
                  .IsRequired(false);


            builder.HasIndex(u => u.UserName)
               .HasDatabaseName("IX_User_UserName")
               .IsUnique(false);

            builder.HasIndex(u => u.NormalizedUserName)
                   .HasDatabaseName("IX_User_NormalizedUserName")
                   .IsUnique(false);

        }

    }
    
}
