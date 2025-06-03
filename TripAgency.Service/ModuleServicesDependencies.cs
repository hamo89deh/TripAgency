using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Implemetations;

namespace TripAgency.Service
{
    public static class ModuleServicesDependencies
    {
        public static IServiceCollection AddServicesDependencies(this IServiceCollection services)
        {          

            services.AddTransient<ICityService,CityService>();
            services.AddTransient<IDestinationService,DestinationService>();
            services.AddTransient<IHotelService,HotelService>();

            return services;
        }
    }
}
