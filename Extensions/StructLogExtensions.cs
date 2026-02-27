using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StructLog.Enrichers;
using StructLog.Interfaces;
using StructLog.Models;

namespace StructLog.Extensions
{
    public static class StructLogExtensions
    {
        public static IServiceCollection AddStructLog(this IServiceCollection services, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(configuration);

            services.Configure<StructLogOptions>(configuration.GetSection("StructLog"));

            services.AddScoped(typeof(IStructLog<>), typeof(StructuredLogger<>));
            services.AddSingleton<ILogEnricher, MachineNameEnricher>();

            return services;
        }
    }
}
