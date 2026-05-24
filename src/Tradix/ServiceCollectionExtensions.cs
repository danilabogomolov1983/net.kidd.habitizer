using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Net.Kidd.Habitizer.Features.Snapshot;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Tradix.Configuration;
using Net.Kidd.Habitizer.Tradix.Snapshot;
using Net.Kidd.Habitizer.Shared.Kernel.Validation;

namespace Net.Kidd.Habitizer.Tradix;

public static class ServiceCollectionExtensions
{
    public static void AddTradix(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IValidator<TradixSettings>, TradixSettingsValidator>();
        services.AddOptions<TradixSettings>()
            .Bind(configuration.GetSection(TradixSettings.ConfigurationSection))
            .ValidateFluentValidation()
            .ValidateOnStart();
        
        services.AddPooledDbContextFactory<SnapshotDbContext>((s, b) =>
        {
            var config = s.GetRequiredService<IOptions<TradixSettings>>().Value;
            var connectionString = config.ConnectionString;

            b.UseSqlServer(connectionString);
        });
        
        services.AddKeyedScoped<IPortfolioSnapshotProvider, SnapshotProvider>(ESnapshotSource.Tradix);
    }
}
