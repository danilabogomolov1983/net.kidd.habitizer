using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Net.Kidd.Habitizer.Persistence.DbContext;

namespace Net.Kidd.Habitizer.Persistence;

public static class ServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PersistenceOptions>(configuration.GetSection("Persistence"));
        services.AddPooledDbContextFactory<PortfolioDbContext>((s, b) =>
        {
            var config = s.GetRequiredService<IOptions<PersistenceOptions>>().Value;
            var connectionString = config.ConnectionString;

            b.UseSqlServer(
                connectionString,
                o => o.MigrationsHistoryTable(Constants.MigrationTableName, Constants.Schema));
        });

        services.AddSingleton<IPortfolioDbContextFactory, PortfolioDbContextFactory>();

        services.AddScoped<Domain.Position.IPersistenceStore, Position.PersistenceStore>();
        services.AddScoped<Domain.Position.IBulkStore, Position.BulkStore>();
        
        services.AddScoped<Domain.Account.IPersistenceStore, Account.PersistenceStore>();
        services.AddScoped<Domain.Account.IBulkStore, Account.BulkStore>();
        services.AddScoped<Domain.Source.IPersistenceStore, Source.PersistenceStore>();
        services.AddScoped<Domain.Source.IBulkStore, Source.BulkStore>();
        services.AddScoped<Domain.Instrument.IPersistenceStore, Instrument.PersistenceStore>();
        services.AddScoped<Domain.Instrument.IBulkStore, Instrument.BulkStore>();

    }
}

