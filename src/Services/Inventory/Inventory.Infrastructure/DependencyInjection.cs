using System;
using Inventory.Core.Interfaces;
using Inventory.Infrastructure.Data;
using Inventory.Infrastructure.Repositories;
using Inventory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventory.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<InventoryContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(InventoryContext).Assembly.FullName)));

            // Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IWarehouseItemRepository, WarehouseItemRepository>();
            services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
            services.AddScoped<IInventoryCountRepository, InventoryCountRepository>();
            services.AddScoped<IInventoryCountItemRepository, InventoryCountItemRepository>();

            // Services
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IInventoryCountService, InventoryCountService>();

            return services;
        }
    }
} 