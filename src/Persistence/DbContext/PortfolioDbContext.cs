using Microsoft.EntityFrameworkCore;
using Net.Kidd.Habitizer.Persistence.Account;
using Net.Kidd.Habitizer.Persistence.Instrument;
using Net.Kidd.Habitizer.Persistence.Position;
using Net.Kidd.Habitizer.Persistence.Source;

namespace Net.Kidd.Habitizer.Persistence.DbContext;

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

