using Microsoft.EntityFrameworkCore;
using Wst.Tools.PosiBridge.Persistence.Account;
using Wst.Tools.PosiBridge.Persistence.Instrument;
using Wst.Tools.PosiBridge.Persistence.Position;
using Wst.Tools.PosiBridge.Persistence.Source;

namespace Wst.Tools.PosiBridge.Persistence.DbContext;

public class PortfolioDbContext(DbContextOptions<PortfolioDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<PositionDbo> Positions => Set<PositionDbo>();
    public DbSet<AccountDbo> Accounts => Set<AccountDbo>();
    public DbSet<InstrumentDbo> Instruments => Set<InstrumentDbo>();
    public DbSet<SourceDbo> Sources => Set<SourceDbo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Constants.Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}

