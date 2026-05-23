using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Domain.Source;
using Net.Kidd.Habitizer.TestCompanion;
using GetPosition = Net.Kidd.Habitizer.Application.Position.Get;
using SaveSnapshot = Net.Kidd.Habitizer.Application.Snapshot.Save;

namespace Net.Kidd.Habitizer.Application.IntegrationTest.Snapshot.Save;

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
