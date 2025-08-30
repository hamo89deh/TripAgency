using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using TripAgency.Api.Behavior;
using TripAgency.Data.Entities.Identity;
using TripAgency.Data.Helping;
using TripAgency.Infrastructure;
using TripAgency.Infrastructure.Context;
using TripAgency.Middleware;
using TripAgency.Service;
using TripAgency.Service.Feature.City.Command.Validaters;
using TripAgency.Service.Mapping.City_Entity;


namespace TripAgency
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {



                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.

                builder.Services.AddControllers(op => op.Filters.Add(typeof(ValidationFilter)))
                    .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = context => null!)
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    });
                ;
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.MapType<TimeSpan>(() => new OpenApiSchema
                    {
                        Type = "string",
                        Example = new OpenApiString("00:00:00")
                    });
                });

                builder.Services.AddDbContext<TripAgencyDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddServicesDependencies(builder.Configuration)
                                .AddInfrastructureDependencies();

                builder.Services.AddAutoMapper(typeof(CityProfile).Assembly);

                builder.Services.AddFluentValidationAutoValidation()
                                .AddValidatorsFromAssembly(typeof(AddCityDtoValidation).Assembly);


                builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
                builder.Services.AddTransient<IUrlHelper>(x =>
                {
                    var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                    var factory = x.GetRequiredService<IUrlHelperFactory>();
                    return factory.GetUrlHelper(actionContext);
                });
                #region addIdentity

                builder.Services.AddIdentity<User, Role>(option =>
                {
                    // Password settings.
                    option.Password.RequireDigit = true;
                    option.Password.RequireLowercase = true;
                    option.Password.RequireNonAlphanumeric = true;
                    option.Password.RequireUppercase = true;
                    option.Password.RequiredLength = 6;
                    option.Password.RequiredUniqueChars = 1;

                    // Lockout settings.
                    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    option.Lockout.MaxFailedAccessAttempts = 5;
                    option.Lockout.AllowedForNewUsers = true;

                    // User settings.
                    option.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    option.User.RequireUniqueEmail = true;
                    option.SignIn.RequireConfirmedEmail = false;
                    

                }).AddEntityFrameworkStores<TripAgencyDbContext>().AddDefaultTokenProviders();
                #endregion

                //Authontication

                builder.Services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
               .AddJwtBearer(x =>
               {
                   x.RequireHttpsMetadata = false;
                   x.SaveToken = true;
                   x.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = Boolean.Parse(builder.Configuration["jwtSettings:ValidateIssuer"]!),
                       ValidIssuers = new[] { builder.Configuration["jwtSettings:Issuer"] },
                       ValidateIssuerSigningKey = Boolean.Parse(builder.Configuration["jwtSettings:ValidateIssuerSigningKey"]!),
                      
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtSettings:Secret"]!)),
                       ValidAudience = builder.Configuration["jwtSettings:Audience"],
                       ValidateAudience = Boolean.Parse(builder.Configuration["jwtSettings:ValidateAudience"]!) ,
                       ValidateLifetime = Boolean.Parse(builder.Configuration["jwtSettings:ValidateLifeTime"]! ),
                   };
               });

                // CORS 
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowReactApp",
                        policy =>
                        {
                            policy.AllowAnyOrigin() // ????? React app ????? ??
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                            //.AllowCredentials(); // ??? ??? ?????? authentication
                        });

                    // ?? ?????? ??? origin (??????? ???)
                    options.AddPolicy("AllowAll",
                        policy =>
                        {
                            policy.AllowAnyOrigin()
                                  .AllowAnyHeader()
                                  .AllowAnyMethod();
                        });
                });
                //Swagger Gn
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Trip Agency", Version = "v1" });
                    // c.EnableAnnotations();

                    c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = JwtBearerDefaults.AuthenticationScheme
                    });

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = JwtBearerDefaults.AuthenticationScheme
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                });
                //Auth Filter
                builder.Services.AddTransient<AuthFilter>();


                var app = builder.Build();
                app.UseMiddleware<ErrorHandlerExceptionMiddleware>();
                
                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                app.UseStaticFiles();
                app.UseHttpsRedirection();
                app.UseCors("AllowReactApp");
                app.UseAuthentication();
                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
