namespace Wst.Tools.PosiBridge.Persistence;

public interface IPortfolioDbContextFactory
{
    Task<DbContext.PortfolioDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
}

