

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TripAgency.Data.Helping;
using TripAgency.Infrastructure.Abstracts;
using TripAgency.Service.Abstracts;
using TripAgency.Service.Implementations;

namespace TripAgency.Service
{
    public static class ModuleServicesDependencies
    {
        public static IServiceCollection AddServicesDependencies(this IServiceCollection services, IConfiguration configuration)
        {          

            services.AddTransient<ICityService,CityService>();
            services.AddTransient<IDestinationService,DestinationService>();
            services.AddTransient<IHotelService,HotelService>();
            services.AddTransient<ITripService, TripService>();
            services.AddTransient<ITypeTripService, TypeTripService>();
            services.AddTransient<IPackageTripService, PackageTripService>();
            services.AddTransient<IPackageTripDestinationService, PackageTripDestinationService>();
            services.AddTransient<IPackageTripDestinationActivityService, PackageTripDestinationActivityService>();
            services.AddTransient<IPackageTripDateService, PackageTripDateService>();
            services.AddTransient<IActivityService, ActivityService>();
            services.AddTransient<IBookingTripService, BookingTripService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddSingleton<IPaymentTimerService, PaymentTimerService>();
            services.AddTransient<IPaymentGatewayService, UsdtPaymentGatewayService>();
            services.AddTransient<IPaymentGatewayService, PayeerPaymentGatewayService>();
            services.AddTransient<IPaymentGatewayFactory, PaymentGatewayFactory>();
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<UsdtPaymentGatewayService>(); 
            services.AddTransient<PayeerPaymentGatewayService>();


            //Email
            var emailSettings = new EmailSettings();
            configuration.GetSection(nameof(emailSettings)).Bind(emailSettings);
            services.AddSingleton(emailSettings);

            return services;
        }
    }
}
