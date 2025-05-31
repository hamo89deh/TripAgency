
using Microsoft.EntityFrameworkCore;
using TripAgency.Infrastructure.Context;
using TripAgency.Infrastructure;
using TripAgency.Service;
using System.Reflection;
using TripAgency.Middleware;
using TripAgency.Api.Behavior;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Data.SqlClient;
using TripAgency.Api.Feature.City.Command.Validaters;


namespace TripAgency
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers( op=> op.Filters.Add(typeof(ValidationFilter)))
                .ConfigureApiBehaviorOptions(options =>options.InvalidModelStateResponseFactory = context => null!)
                              ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<TripAgencyDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddServicesDependencies()
                            .AddInfrastructureDependencies();

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            builder.Services.AddFluentValidationAutoValidation()
                            .AddValidatorsFromAssembly(typeof(AddCityDtoValidation).Assembly);

            var app = builder.Build();
            app.UseMiddleware<ErrorHandlerExceptionMiddleware>();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
