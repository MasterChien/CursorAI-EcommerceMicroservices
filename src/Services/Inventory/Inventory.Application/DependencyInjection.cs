using System.Reflection;
using FluentValidation;
using Inventory.Application.Behaviors;
using Inventory.Application.Mappings;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Đăng ký AutoMapper
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Đăng ký MediatR
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            // Đăng ký FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
} 