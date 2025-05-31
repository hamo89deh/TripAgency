using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class CityConfigurations : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x=>x.Name)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(50)
                   .IsRequired(true);

            builder.ToTable("Cities");

        }

    }
}
