using System.Collections.Immutable;
using Wst.Tools.PosiBridge.Domain.Snapshot;

namespace Wst.Tools.PosiBridge.Domain.Test.Snapshot;

public class SnapshotExtensionsTests
{
    [Fact]
    public void WithPositions()
    {
        var faker = NewFaker();
        var account = NewAccount();
        var source = faker.PickRandom<ESnapshotSource>();
        var snapshot = NewSnapshot(source, account, 3);
        var positions = ImmutableList.Create(
            NewPosition(),
            NewPosition());

        var actual = snapshot.WithPositions(positions);

        Assert.Equal(positions, actual.Positions);
        Assert.NotSame(snapshot, actual);
    }
    [Fact]
    public void WithSource()
    {
        var faker = NewFaker();
        var account = NewAccount();
        var source = faker.PickRandom<ESnapshotSource>();
        var snapshot = NewSnapshot(source, account, 3);

        var expectedSource = faker.PickRandom<ESnapshotSource>();
        var actual = snapshot.WithSource(expectedSource);

        Assert.Equal(expectedSource, actual.Source);
        Assert.NotSame(snapshot, actual);
    }
}
