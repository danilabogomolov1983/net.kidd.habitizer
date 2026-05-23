using Microsoft.EntityFrameworkCore;
using Net.Kidd.Habitizer.Tradix.Snapshot.Position;

namespace Net.Kidd.Habitizer.Tradix.Snapshot;

public class SnapshotDbContext(DbContextOptions<SnapshotDbContext> options): DbContext(options)
{
    public DbSet<PositionDbo> Positions => Set<PositionDbo>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SnapshotDbContext).Assembly);
    }
}
