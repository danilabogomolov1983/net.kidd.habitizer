namespace Net.Kidd.Habitizer.Persistence;

public interface IPortfolioDbContextFactory
{
    Task<DbContext.PortfolioDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
}

