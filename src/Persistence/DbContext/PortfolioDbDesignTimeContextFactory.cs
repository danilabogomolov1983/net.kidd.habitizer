using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Wst.Tools.PosiBridge.Persistence.DbContext;

public class PortfolioDbDesignTimeContextFactory : IDesignTimeDbContextFactory<PortfolioDbContext>
{
    public PortfolioDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PortfolioDbContext>();
        ((DbContextOptionsBuilder)optionsBuilder).UseSqlServer(Constants.GetConnectionString(),
            o => o.MigrationsHistoryTable(Constants.MigrationTableName, Constants.Schema));

        return new PortfolioDbContext(optionsBuilder.Options);
    }
}

