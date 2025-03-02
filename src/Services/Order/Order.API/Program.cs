using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Order.Application;
using Order.Infrastructure;
using Serilog;
using System.IO.Compression;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Cấu hình Kestrel cho hiệu suất cao
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 1000;
    options.Limits.MaxConcurrentUpgradedConnections = 1000;
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10 MB
    options.Limits.MinRequestBodyDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
    options.Limits.MinResponseDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
    options.ListenAnyIP(5004); // Lắng nghe trên cổng 5004
});

// Thêm các dịch vụ vào container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Thêm các dịch vụ từ Application và Infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Cấu hình Response Compression
if (builder.Configuration.GetValue<bool>("PerformanceSettings:EnableCompression"))
{
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<GzipCompressionProvider>();
    });

    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest;
    });
}

// Cấu hình Rate Limiting
if (builder.Configuration.GetValue<bool>("PerformanceSettings:EnableRateLimiting"))
{
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<Microsoft.AspNetCore.Http.HttpContext, string>(httpContext =>
            RateLimitPartition.GetFixedWindowLimiter("GlobalLimiter", _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = builder.Configuration.GetValue<int>("PerformanceSettings:MaxRequestsPerMinute"),
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 100
            }));
    });
}

// Cấu hình Output Caching
if (builder.Configuration.GetValue<bool>("PerformanceSettings:EnableCaching"))
{
    builder.Services.AddOutputCache(options =>
    {
        options.DefaultExpirationTimeSpan = TimeSpan.FromMinutes(
            builder.Configuration.GetValue<int>("PerformanceSettings:CacheExpirationMinutes"));
        options.AddBasePolicy(builder => builder.Cache());
    });
}

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Cấu hình HTTP request pipeline
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("ApiSettings:EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Sử dụng Response Compression nếu được bật
if (builder.Configuration.GetValue<bool>("PerformanceSettings:EnableCompression"))
{
    app.UseResponseCompression();
}

// Sử dụng Rate Limiting nếu được bật
if (builder.Configuration.GetValue<bool>("PerformanceSettings:EnableRateLimiting"))
{
    app.UseRateLimiter();
}

// Sử dụng Output Caching nếu được bật
if (builder.Configuration.GetValue<bool>("PerformanceSettings:EnableCaching"))
{
    app.UseOutputCache();
}

app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Starting Order API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Order API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
