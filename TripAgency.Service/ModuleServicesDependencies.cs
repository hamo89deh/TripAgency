using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Implementations;

namespace TripAgency.Service
{
    public static class ModuleServicesDependencies
    {
        public static IServiceCollection AddServicesDependencies(this IServiceCollection services)
        {          

            services.AddTransient<ICityService,CityService>();
            services.AddTransient<IDestinationService,DestinationService>();
            services.AddTransient<IHotelService,HotelService>();
            services.AddTransient<ITripService, TripService>();
            services.AddTransient<ITypeTripService, TypeTripService>();
            services.AddTransient<IPackageTripService, PackageTripService>();
            services.AddTransient<IPackageTripDestinationService, PackageTripDestinationService>();
            services.AddTransient<IPackageTripDestinationActivityService, PackageTripDestinationActivityService>();
            services.AddTransient<IActivityService, ActivityService>();
            services.AddTransient<ITripDateService, TripDateService>();
            services.AddTransient<IBookingTripService, BookingTripService>();

            return services;
        }
    }
}
