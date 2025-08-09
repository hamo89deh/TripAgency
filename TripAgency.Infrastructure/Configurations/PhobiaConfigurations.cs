using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class PhobiaConfigurations : IEntityTypeConfiguration<Phobia>
    {
        public void Configure(EntityTypeBuilder<Phobia> builder)
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
        
            builder.ToTable("Phobias");

        }

    }

}
