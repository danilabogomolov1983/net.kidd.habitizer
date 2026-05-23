using LanguageExt.UnsafeValueAccess;
using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Domain.Snapshot;
using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.TestCompanion;
using GetPosition = Wst.Tools.PosiBridge.Application.Position.Get;
using SaveSnapshot = Wst.Tools.PosiBridge.Application.Snapshot.Save;

namespace Wst.Tools.PosiBridge.Application.IntegrationTest.Snapshot.Save;

[Collection("IntegrationTests")]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly SaveSnapshot.Service _saveService = fixture.GetRequiredService<SaveSnapshot.Service>();
    private readonly GetPosition.Service _getPositionService = fixture.GetRequiredService<GetPosition.Service>();

    [Fact]
    public async Task Save_SavesAllSnapshotPositionsForAllMappedAccounts()
    {
        var faker = NewFaker();
        var source = faker.PickRandom<ESnapshotSource>();
        var account = NewAccount().WithSource(NewSource().WithName(source.ToSourceName()));
        var expected = NewSnapshot(source, account, 2);

        var maybeSave = await _saveService.SaveAsync(new SaveSnapshot.Command(expected));

        Assert.True(maybeSave.IsSucc);

        foreach (var expectedPosition in expected.Positions)
        {
            var getCommand = new GetPosition.Command(
                expectedPosition.Account.Source.Name,
                expectedPosition.Account.Name,
                expectedPosition.Instrument.Isin);

            var maybePosition = await _getPositionService.GetAsync(getCommand);
            Assert.True(maybePosition.IsSucc);

            var actualPosition = maybePosition.ToOption().ValueUnsafe();
            Assert.NotNull(actualPosition);

            Assert.EquivalentWithExclusions(expectedPosition, actualPosition, 
                x => x.Account.Id,
                x => x.Account.Source.Id,
                x => x.Instrument.Id);
        }
    }
}
