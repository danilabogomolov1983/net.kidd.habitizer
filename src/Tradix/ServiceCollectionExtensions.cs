using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wst.Tools.PosiBridge.Application.Snapshot;
using Wst.Tools.PosiBridge.Domain.Snapshot;
using Wst.Tools.PosiBridge.Tradix.Configuration;
using Wst.Tools.PosiBridge.Tradix.Snapshot;
using Wst.Tools.PosiBridge.Shared.Kernel.Validation;

namespace Wst.Tools.PosiBridge.Tradix;

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
