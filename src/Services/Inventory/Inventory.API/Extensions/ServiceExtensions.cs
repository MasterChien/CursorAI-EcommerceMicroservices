using Microsoft.OpenApi.Models;

namespace Inventory.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Inventory API",
                    Version = "v1",
                    Description = "API quản lý kho hàng cho hệ thống thương mại điện tử",
                    Contact = new OpenApiContact
                    {
                        Name = "Inventory Team",
                        Email = "inventory@example.com"
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddCorsServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });

            return services;
        }
    }
} 