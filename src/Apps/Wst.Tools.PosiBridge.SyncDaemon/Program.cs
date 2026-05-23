using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using Serilog;
using Wst.Tools.PosiBridge.Application;
using Wst.Tools.PosiBridge.Persistence;
using Wst.Tools.PosiBridge.TickTs;
using Wst.Tools.PosiBridge.Tradix;
using Wst.Tools.PosiBridge.SyncDaemon.Workers;

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
            var tradixJobKey = new JobKey(nameof(TradixSyncWorker));
            var tickTsJobKey = new JobKey(nameof(TickTsSyncWorker));

            Log.Information("[Module:SyncDaemon.Bootstrap][Step:RegisterModule] Registering Application module");
            services.AddApplication(context.Configuration);
            
            Log.Information("[Module:SyncDaemon.Bootstrap][Step:RegisterModule] Registering Persistence module");
            services.AddPersistence(context.Configuration);
            
            Log.Information("[Module:SyncDaemon.Bootstrap][Step:RegisterModule] Registering Tradix module");
            services.AddTradix(context.Configuration);
            
            Log.Information("[Module:SyncDaemon.Bootstrap][Step:RegisterModule] Registering TickTs module");
            services.AddTickTs(context.Configuration);
            
            Log.Information("[Module:SyncDaemon.Bootstrap][Step:RegisterScheduler] Registering Quartz jobs");
            services.AddQuartz(q =>
            {
                q.AddJob<TradixSyncWorker>(options => options.WithIdentity(tradixJobKey));
                q.AddTrigger(options => options
                    .WithIdentity($"{nameof(TradixSyncWorker)}StartupTrigger")
                    .ForJob(tradixJobKey)
                    .StartNow());
                q.AddTrigger(options => options
                    .WithIdentity($"{nameof(TradixSyncWorker)}Trigger")
                    .ForJob(tradixJobKey)
                    .WithCronSchedule("0 5/10 * * * ?"));

                q.AddJob<TickTsSyncWorker>(options => options.WithIdentity(tickTsJobKey));
                q.AddTrigger(options => options
                    .WithIdentity($"{nameof(TickTsSyncWorker)}StartupTrigger")
                    .ForJob(tickTsJobKey)
                    .StartNow());
                q.AddTrigger(options => options
                    .WithIdentity($"{nameof(TickTsSyncWorker)}Trigger")
                    .ForJob(tickTsJobKey)
                    .WithCronSchedule("0 5/10 * * * ?"));
            });
            services.AddQuartzHostedService(options =>
            {
                options.WaitForJobsToComplete = true;
            });
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
