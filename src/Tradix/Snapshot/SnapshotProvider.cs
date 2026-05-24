using LanguageExt;
using LanguageExt.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using Net.Kidd.Habitizer.Features.Snapshot;
using Net.Kidd.Habitizer.Domain.Position;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Domain.Source;

namespace Net.Kidd.Habitizer.Tradix.Snapshot;

public sealed class SnapshotProvider(IDbContextFactory<SnapshotDbContext> dbContextFactory) : IPortfolioSnapshotProvider
{
    public static readonly Source TradixSource = new(SourceId.Empty(), ESnapshotSource.Tradix.ToSourceName());

    public async Task<Fin<Domain.Snapshot.Snapshot>> GetAsync(string account)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var positions = await dbContext.Positions
                .AsNoTracking()
                .Where(i => i.Account == account)
                .ToListAsync();

            var mappedPositions = positions
                .Map(position => TradixMappers.SnapshotPositionMapper.Map((TradixSource, position)))
                .Where(i => i.IsValid())
                .ToImmutableList();

            return Fin<Domain.Snapshot.Snapshot>.Succ(new Domain.Snapshot.Snapshot(ESnapshotSource.Tradix, mappedPositions));
        }
        catch (Exception ex)
        {
            return Error.New("Failed to read Tradix snapshot positions.", ex);
        }
    }

    public async Task<Fin<Domain.Snapshot.Snapshot>> GetAsync(string[] accounts)
    {
        try
        {
            await using var dbContext = await dbContextFactory.CreateDbContextAsync();
            var positions = await dbContext.Positions
                .AsNoTracking()
                .Where(i => accounts.AsEnumerable().Contains(i.Account))
                .ToListAsync();

            var mappedPositions = positions
                .Map(position => TradixMappers.SnapshotPositionMapper.Map((TradixSource, position)))
                .Where(i => i.IsValid())
                .ToImmutableList();

            return Fin<Domain.Snapshot.Snapshot>.Succ(new Domain.Snapshot.Snapshot(ESnapshotSource.Tradix, mappedPositions));
        }
        catch (Exception ex)
        {
            return Error.New("Failed to read Tradix snapshot positions.", ex);
        }
    }
}
