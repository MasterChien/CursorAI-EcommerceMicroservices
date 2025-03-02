using Order.Core.StateMachine;

namespace Order.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Đăng ký DbContext với SQL Server
            services.AddDbContext<OrderDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("OrderConnectionString"),
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        // Cấu hình cho khả năng phục hồi kết nối
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
                
                // Đặt ExecutionStrategy mặc định cho DbContext
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);
            }, ServiceLifetime.Scoped);

            // Đăng ký Repository và UnitOfWork
            services.AddScoped(typeof(IRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IOrderRepository, OrderRepository>();
            
            // Đăng ký OrderStateMachine
            services.AddScoped<IOrderStateMachine, OrderStateMachine>();
            
            // Đăng ký UnitOfWork sau khi đã đăng ký các dependencies
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
} 