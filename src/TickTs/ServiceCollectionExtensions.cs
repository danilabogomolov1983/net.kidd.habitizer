using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wst.Tools.PosiBridge.Application.Snapshot;
using Wst.Tools.PosiBridge.Domain.Snapshot;
using Wst.Tools.PosiBridge.TickTs.Configuration;
using Wst.Tools.PosiBridge.TickTs.Http;
using Wst.Tools.PosiBridge.TickTs.Snapshot;
using Wst.Tools.PosiBridge.Shared.Kernel.Validation;

namespace Wst.Tools.PosiBridge.TickTs;

public static class ServiceCollectionExtensions
{
    public static void AddTickTs(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddScoped<IValidator<TickTsSettings>, TickTsSettingsValidator>();
        services.AddOptions<TickTsSettings>()
            .Bind(configuration.GetSection(TickTsSettings.ConfigurationSection))
            .ValidateFluentValidation()
            .ValidateOnStart();
        
        services
            .AddHttpClient<ITickTsHttpClient, TickTsHttpClient>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<TickTsSettings>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5))
            .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<TickTsSettings>>().Value;
                return TickTsHttpHandlerFactory.Create(options);
            });

        services.AddKeyedScoped<IPortfolioSnapshotProvider, SnapshotProvider>(ESnapshotSource.TickTs);
    }
}
