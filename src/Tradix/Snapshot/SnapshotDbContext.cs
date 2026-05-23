using Microsoft.EntityFrameworkCore;
using Wst.Tools.PosiBridge.Tradix.Snapshot.Position;

namespace Wst.Tools.PosiBridge.Tradix.Snapshot;

public class SnapshotDbContext(DbContextOptions<SnapshotDbContext> options): DbContext(options)
{
    public DbSet<PositionDbo> Positions => Set<PositionDbo>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SnapshotDbContext).Assembly);
    }
}
