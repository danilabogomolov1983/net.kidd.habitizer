using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Persistence.Position;
using Net.Kidd.Habitizer.TestCompanion;
using GetPosition = Net.Kidd.Habitizer.Application.Position.Get;
using PutPosition = Net.Kidd.Habitizer.Application.Position.Put;

namespace Net.Kidd.Habitizer.Application.Test.Position.Put;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PutPosition.Service _putService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Application.Account.Post.Service(new Persistence.Account.PersistenceStore(fixture.ContextFactory), new Application.Source.Post.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))),
        new Application.Instrument.Post.Service(new Persistence.Instrument.PersistenceStore(fixture.ContextFactory))
    );
    private readonly GetPosition.Service _getService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Application.Account.Get.Service(new Persistence.Account.PersistenceStore(fixture.ContextFactory), new Application.Source.Get.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory))),
        new Application.Instrument.Get.Service(new Persistence.Instrument.PersistenceStore(fixture.ContextFactory))
    );

    [Fact]
    public async Task Put_Get_Success()
    {
        var position = NewPosition();
        var putCommand = NewCommand(position);

        var maybePut = await _putService.PutAsync(putCommand);
        Assert.True(maybePut.IsSucc);
        var expected = maybePut.ToOption().ValueUnsafe();

        var getCommand = new GetPosition.Command(expected.Account.Source.Name, expected.Account.Name, expected.Instrument.Isin);
        var maybePosition = await _getService.GetAsync(getCommand);
        Assert.True(maybePosition.IsSucc);
        var actual = maybePosition.ToOption().ValueUnsafe();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Put_ExistingPosition_Updates()
    {
        var initial = NewPosition();
        var updated = initial with
        {
            NetSize = initial.NetSize + 1,
            NetValue = initial.NetValue + 2,
            UnrealisedAverageCost = initial.UnrealisedAverageCost + 3,
            UnrealisedProfit = initial.UnrealisedProfit + 4,
            UnrealisedProfitPercent = initial.UnrealisedProfitPercent + 5,
            ReferencePrice = NewReferencePrice()
        };

        var firstPut = await _putService.PutAsync(NewCommand(initial));
        Assert.True(firstPut.IsSucc);

        var secondPut = await _putService.PutAsync(NewCommand(updated));
        Assert.True(secondPut.IsSucc);

        var expected = secondPut.ToOption().ValueUnsafe();
        var getCommand = new GetPosition.Command(expected.Account.Source.Name, expected.Account.Name, expected.Instrument.Isin);
        var maybePosition = await _getService.GetAsync(getCommand);
        Assert.True(maybePosition.IsSucc);
        var actual = maybePosition.ToOption().ValueUnsafe();

        Assert.NotEqual(initial, actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Put_Duplicate_IsIdempotent()
    {
        var position = NewPosition();
        var command = NewCommand(position);

        var firstPut = await _putService.PutAsync(command);
        Assert.True(firstPut.IsSucc);

        var secondPut = await _putService.PutAsync(command);
        Assert.True(secondPut.IsSucc);

        var expected = firstPut.ToOption().ValueUnsafe();
        var actual = secondPut.ToOption().ValueUnsafe();

        Assert.Equal(expected, actual);
    }

    private static PutPosition.Command NewCommand(Domain.Position.Position position) =>
        new(
            position.Account.Source.Name,
            position.Account.Name,
            position.Instrument.Isin,
            position.NetSize,
            position.NetValue,
            position.UnrealisedAverageCost,
            position.UnrealisedProfit,
            position.UnrealisedProfitPercent,
            position.ReferencePrice);
}
