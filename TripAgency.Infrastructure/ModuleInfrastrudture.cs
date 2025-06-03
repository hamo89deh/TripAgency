using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Infrastructure.InfrastructureBases;
using TripAgency.Infrastructure.Repositories;

namespace TripAgency.Infrastructure
{
    public static class ModuleInfrastrudtureDepndencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<ICityRepositoryAsync, CityRepositoryAsync>();
            services.AddTransient<IDestinationRepositoryAsync, DestinationRepositoryAsync>();
            services.AddTransient < IHotelRepositoryAsync, HotelRepositoryAsync>();
            

            return services;
        }
            
    }
}
