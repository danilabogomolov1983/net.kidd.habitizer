using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Net.Kidd.Habitizer.Persistence.DbContext;

public class PortfolioDbContextFactory : IPortfolioDbContextFactory
{
    private readonly IDbContextFactory<PortfolioDbContext> _factory;

    public PortfolioDbContextFactory(IDbContextFactory<PortfolioDbContext> fact)
    {
        _factory = fact;

        using (Activity.Current?.Source.StartActivity(string.Join(".", nameof(PortfolioDbContextFactory), "Migrate")))
        {
            using var context = _factory.CreateDbContext();

            if (context.Database.IsRelational())
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            }
        }
    }


    public Task<PortfolioDbContext> CreateDbContextAsync(CancellationToken cancellationToken) => _factory.CreateDbContextAsync(cancellationToken);
}

