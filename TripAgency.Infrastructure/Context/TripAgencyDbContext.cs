using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public DbSet<PackageTrip> PackageTrips { get; set; }
        public DbSet<PackageTripType> PackageTripTypes { get; set; }
        public DbSet<PackageTripDestination> PackageTripDestinations { get; set; }
        public DbSet<PackageTripDestinationActivity> PackageTripDestinationActivities { get; set; }
        public DbSet<Data.Entities.Activity> Activities { get; set; }
        public DbSet<TripDate> TripDates { get; set; }
        public DbSet<TripDestination> TripDestinations { get; set; }
        public DbSet<DestinationActivity> DestinationActivities { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BookingTrip> BookingTrips { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
