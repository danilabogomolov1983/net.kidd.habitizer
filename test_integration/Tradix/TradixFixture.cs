using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Testcontainers.MsSql;
using Net.Kidd.Habitizer.Application.Snapshot;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Tradix.Configuration;
using Net.Kidd.Habitizer.Tradix.Snapshot;

namespace Net.Kidd.Habitizer.Tradix.IntegrationTest;

public sealed class TradixFixture : IAsyncLifetime
{
    private const string ConnectionStringKey = $"{TradixSettings.ConfigurationSection}:ConnectionString";
    private ServiceProvider? _serviceProvider;
    private MsSqlContainer? _dbContainer;
    
    public IPortfolioSnapshotProvider SnapshotProvider { get; private set; } = default!;
    public IDbContextFactory<SnapshotDbContext> SnapshotDbContextFactory { get; private set; } = default!;
    

    public async ValueTask InitializeAsync()
    {
        var useContainer = ShouldUseContainerFromTestSettings();

        if (useContainer)
        {
            _dbContainer = Infrastructure.UseContainer();
            await _dbContainer.StartAsync();
            _serviceProvider = BuildServiceProvider(InMemoryConfig(_dbContainer.GetConnectionString()));
        }
        else
        {
            _serviceProvider = BuildServiceProvider(FileConfig);
        }

        SnapshotDbContextFactory = _serviceProvider.GetRequiredService<IDbContextFactory<SnapshotDbContext>>();
        await using var dbContext = await SnapshotDbContextFactory.CreateDbContextAsync(TestContext.Current.CancellationToken);
        await Support.Infrastructure.EnsureSchemaAsync(dbContext);

        SnapshotProvider = _serviceProvider.GetRequiredKeyedService<IPortfolioSnapshotProvider>(ESnapshotSource.Tradix);

    }

    // public T GetRequiredService<T>() where T : notnull
    // {
    //     ArgumentNullException.ThrowIfNull(_serviceProvider);
    //     return _serviceProvider.GetRequiredService<T>();
    // }
    // public IPortfolioSnapshotProvider GetSnapshotProvider()
    // {
    //     ArgumentNullException.ThrowIfNull(_serviceProvider);
    //     return _serviceProvider.GetKeyedService<IPortfolioSnapshotProvider>(ESnapshotSource.Tradix) ??
    //            throw new NotFoundException();
    // }

    public async ValueTask DisposeAsync()
    {
        if (_serviceProvider != null)
        {
            await _serviceProvider.DisposeAsync();
        }

        if (_dbContainer != null)
        {
            await _dbContainer.DisposeAsync();
        }

        await Log.CloseAndFlushAsync();
    }

    private static bool ShouldUseContainerFromTestSettings()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.test.json", optional: true)
            .Build();

        return string.IsNullOrWhiteSpace(configuration[ConnectionStringKey]);
    }

    private static Func<string, Func<IConfigurationRoot>> InMemoryConfig => connectionString
        => () => new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [ConnectionStringKey] = connectionString
            })
            .Build();

    private static Func<IConfigurationRoot> FileConfig => () => new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile("appsettings.test.json", optional: true)
        .Build();

    private ServiceProvider BuildServiceProvider(Func<IConfigurationRoot> configurationFunc)
    {
        var configuration = configurationFunc();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        var services = new ServiceCollection();
        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(Log.Logger, dispose: false);
        });
        services.AddOptions();
        services.AddTradix(configuration);

        return services.BuildServiceProvider();
    }
}
