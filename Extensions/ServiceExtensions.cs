using ShopZone.Data;
using ShopZone.Services;
using ShopZone.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ShopZone.Data;
using ShopZone.Services.Interfaces;
using ShopZone.Services;
using System.Text;

namespace ShopZone.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Generic Repository
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Application Services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISmsService, SmsService>();

            return services;
        }

        public static IServiceCollection AddCorsServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            return services;
        }
    }
}