using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripAgency.Data.Entities;
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
            services.AddTransient< ICityRepositoryAsync, CityRepositoryAsync>();
            services.AddTransient< IDestinationRepositoryAsync, DestinationRepositoryAsync>();
            services.AddTransient < IHotelRepositoryAsync, HotelRepositoryAsync>();
            services.AddTransient < ITripRepositoryAsync, TripRepositoryAsync>();
            services.AddTransient < ITypeTripRepositoryAsync, TypeTripRepositoryAsync>();
            services.AddTransient < IActivityRepositoryAsync, ActivityRepositoryAsync>();
            services.AddTransient < IPackageTripRepositoryAsync, PackageTripRepositoryAsync>();
            services.AddTransient < IPackageTripDestinationActivityRepositoryAsync, PackageTripDestinationActivityRepositoryAsync>();
            services.AddTransient < IPackageTripDestinationRepositoryAsync, PackageTripDestinationRepositoryAsync>();
            services.AddTransient < IPackageTripTypeRepositoryAsync, PackageTripTypeRepositoryAsync>();
            services.AddTransient < ITripDateRepositoryAsync, TripDateRepositoryAsync>();
            services.AddTransient < ITripDestinationRepositoryAsync, TripDestinationRepositoryAsync>();
            services.AddTransient < IDestinationActivityRepositoryAsync, DestinationActivityRepositoryAsync>();
            

            return services;
        }
            
    }
}
