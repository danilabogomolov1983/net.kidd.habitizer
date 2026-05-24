using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Net.Kidd.Habitizer.Features;
using Net.Kidd.Habitizer.Persistence;

var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
var bootstrapConfig = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true);

if (!string.IsNullOrWhiteSpace(environmentName))
{
    bootstrapConfig.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
}

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(bootstrapConfig.Build())
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog((context, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();
        })
        .ConfigureServices((context, services) =>
        {
            Log.Information("[Module:SyncDaemon.Bootstrap][Step:RegisterModule] Registering Application module");
            services.AddApplication(context.Configuration);

            Log.Information("[Module:SyncDaemon.Bootstrap][Step:RegisterModule] Registering Persistence module");
            services.AddPersistence(context.Configuration);
        })
        .Build();

    var logger = host.Services
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("SyncDaemon");

    var environment = environmentName ?? "Unknown";
    logger.LogInformation("Start SyncDaemon. Env: {Environment}", environment);

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SyncDaemon terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
