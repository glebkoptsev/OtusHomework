using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OtusHomework.Database;

namespace OtusHomework.СacheWarmup
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var app = CreateHostBuilder(args);
            using var scope = app.Services.CreateScope();
            var generator = scope.ServiceProvider.GetRequiredService<CacheWormuper>();
            await generator.WarmupAsync();
        }

        public static IHost CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<NpgsqlService>();
                    services.AddScoped<CacheWormuper>();
                    services.AddStackExchangeRedisCache(options =>
                    {
#if DEBUG
                        options.Configuration = hostContext.Configuration.GetConnectionString("redis_debug");
#else
                        options.Configuration = hostContext.Configuration.GetConnectionString("redis");
#endif
                    });
                }).Build();
        }
    }
}
