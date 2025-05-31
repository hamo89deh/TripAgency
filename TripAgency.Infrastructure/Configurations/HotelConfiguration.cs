using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(h => h.Email)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.Location)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.Rate)
                .HasColumnType("tinyint")
                .IsRequired(); 

            builder.Property(h => h.MidPriceForOneNight)
                .IsRequired();

            builder.ToTable("Hotels"); 

           
            builder.HasOne(c=>c.City) 
                   .WithMany(h=>h.Hotels) 
                   .HasForeignKey(h => h.CityId) 
                   .OnDelete(DeleteBehavior.Restrict); 
                                                       
        }
    }
}
