using FakeItEasy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Net.Kidd.Habitizer.Features.Snapshot;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.TickTs.Http;

namespace Net.Kidd.Habitizer.TickTs.IntegrationTest;

public sealed class TickTsFixture : IAsyncLifetime
{
    private const string TestSettingsFileName = "appsettings.test.json";
    private ServiceProvider? _serviceProvider;
    
    public IPortfolioSnapshotProvider SnapshotProvider { get; private set; }

    public ValueTask InitializeAsync()
    {
        _serviceProvider = BuildServiceProvider();
        SnapshotProvider = _serviceProvider.GetRequiredKeyedService<IPortfolioSnapshotProvider>(ESnapshotSource.TickTs);
        return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        if (_serviceProvider != null)
        {
            await _serviceProvider.DisposeAsync();
        }

        await Log.CloseAndFlushAsync();
    }

    private ServiceProvider BuildServiceProvider()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var hasTestSettings = File.Exists(Path.Combine(currentDirectory, TestSettingsFileName));
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(currentDirectory)
            .AddJsonFile("appsettings.json")
            .AddJsonFile(TestSettingsFileName, optional: true);

        if (!hasTestSettings)
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["TickTsSettings:BaseUrl"] = "https://localhost",
                ["TickTsSettings:Token"] = "fake-token",
                ["TickTsSettings:ResolvedAddress"] = "127.0.0.1",
                ["TickTsSettings:TimeoutSeconds"] = "30"
            });
        }

        var configuration = configurationBuilder.Build();

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

        services.AddTickTs(configuration);

        if (!hasTestSettings)
        {
 
            services.RemoveAll<ITickTsHttpClient>();
            services.AddSingleton<ITickTsHttpClient>(serviceProvider => FakedTickTsClient());
        }

        return services.BuildServiceProvider();
    }
    
    private ITickTsHttpClient FakedTickTsClient()
    {
        var result = A.Fake<ITickTsHttpClient>();

        A.CallTo(() => result.SendAsync(A<TickTsHttpRequestPayload>.Ignored))

            .ReturnsLazily((TickTsHttpRequestPayload input) =>
                input.AccountId == Account.Empty().Name
                    ? HttpResponseMessage_AccountNotFound()
                    : HttpResponseMessage_OK(Support.TickTs.SnapshotResponse(input.AccountId)));
        
        return result;
    }
    
    private static HttpResponseMessage HttpResponseMessage_OK(TickTs.Snapshot.Response snapshot)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new
                {
                    positions = snapshot.Positions.Map(Position)
                },
                options: new JsonSerializerOptions(JsonSerializerDefaults.Web))
        };

        dynamic Position(TickTs.Snapshot.Position.Response position)
        {
            return new
            {
                id = position.Id,
                instrumentId = position.InstrumentId,
                netSize = position.NetSize,
                netValue = position.NetValue,
                unrealisedAverageCost = position.UnrealisedAverageCost,
                unrealisedProfit = position.UnrealisedProfit,
                unrealisedProfitPercent = position.UnrealisedProfitPercent,
                referencePrice = position.ReferencePrice != null ? new
                {
                    price = position.ReferencePrice?.Price,
                    currencyId = position.ReferencePrice?.CurrencyId,
                    exchangeId = position.ReferencePrice?.ExchangeId,
                    currencySpot = position.ReferencePrice?.CurrencySpot
                }: null

            };
        }
    }
    private static HttpResponseMessage HttpResponseMessage_BadRequest() => new(HttpStatusCode.BadRequest);
    private static HttpResponseMessage HttpResponseMessage_NullJson()
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };
    }

    private static HttpResponseMessage HttpResponseMessage_AccountNotFound()
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = 
                JsonContent.Create(new { positions = new List<TickTs.Snapshot.Position.Response>() },
                options: new JsonSerializerOptions(JsonSerializerDefaults.Web))
        };

    }
}
