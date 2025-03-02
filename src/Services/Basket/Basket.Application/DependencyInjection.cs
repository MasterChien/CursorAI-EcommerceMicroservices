using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Basket.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
            return services;
        }
    }
} 