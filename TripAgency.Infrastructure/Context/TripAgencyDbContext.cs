using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;

namespace TripAgency.Infrastructure.Context
{
    public class TripAgencyDbContext : DbContext
    {
        public TripAgencyDbContext(DbContextOptions options)  : base(options)
        {
            
        }
        
        public DbSet<City> Cities { get; set; }

        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TypeTrip> TypeTrips { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
