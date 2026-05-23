using LanguageExt.UnsafeValueAccess;
using Net.Kidd.Habitizer.Domain.Account;
using Net.Kidd.Habitizer.Persistence.Account;
using Net.Kidd.Habitizer.TestCompanion;
using GetAccount = Net.Kidd.Habitizer.Application.Account.Get;
using PutAccount = Net.Kidd.Habitizer.Application.Account.Put;
using GetSource = Net.Kidd.Habitizer.Application.Source.Get;
using PostSource = Net.Kidd.Habitizer.Application.Source.Post;

namespace Net.Kidd.Habitizer.Application.Test.Account.Put;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly PutAccount.Service _putService = new(
        new PersistenceStore(fixture.ContextFactory),
        new GetSource.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory)));

    private readonly GetAccount.Service _getService = new(
        new PersistenceStore(fixture.ContextFactory),
        new Application.Source.Get.Service(new Persistence.Source.PersistenceStore(fixture.ContextFactory)));

    private readonly PostSource.Service _postSourceService = new(new Persistence.Source.PersistenceStore(fixture.ContextFactory));

    [Fact]
    public async Task Put_Get_Success()
    {
        var source = NewSource();
        Assert.True((await _postSourceService.PostAsync(new PostSource.Command(source.Name))).IsSucc);
        
        var account = NewAccount().WithSource(source).WithLastUpdatedAt(DateTimeOffset.UtcNow);
        var putCommand = NewCommand(account);

        var maybePut = await _putService.PutAsync(putCommand);
        Assert.True(maybePut.IsSucc);
        var expected = maybePut.ToOption().ValueUnsafe();

        var getCommand = new GetAccount.Command(expected.Source.Name, expected.Name);
        var maybeAccount = await _getService.GetAsync(getCommand);
        Assert.True(maybeAccount.IsSucc);
        var actual = maybeAccount.ToOption().ValueUnsafe();

        Assert.NotNull(actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Put_ExistingAccount_Updates()
    {
        var source = NewSource();
        Assert.True((await _postSourceService.PostAsync(new PostSource.Command(source.Name))).IsSucc);

        var initial = NewAccount().WithSource(source).WithLastUpdatedAt(DateTimeOffset.UtcNow.AddMinutes(-10));
        var updated = initial.WithLastUpdatedAt(DateTimeOffset.UtcNow);
        
        var firstPut = await _putService.PutAsync(NewCommand(initial));
        Assert.True(firstPut.IsSucc);

        var secondPut = await _putService.PutAsync(NewCommand(updated));
        Assert.True(secondPut.IsSucc);

        var expected = secondPut.ToOption().ValueUnsafe();
        var getCommand = new GetAccount.Command(expected.Source.Name, expected.Name);
        var maybeAccount = await _getService.GetAsync(getCommand);
        Assert.True(maybeAccount.IsSucc);
        var actual = maybeAccount.ToOption().ValueUnsafe();

        Assert.NotEqual(initial, actual);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task Put_Duplicate_IsIdempotent()
    {
        var source = NewSource();
        Assert.True((await _postSourceService.PostAsync(new PostSource.Command(source.Name))).IsSucc);

        var account = NewAccount().WithSource(source).WithLastUpdatedAt(DateTimeOffset.UtcNow);
        var command = NewCommand(account);

        var firstPut = await _putService.PutAsync(command);
        Assert.True(firstPut.IsSucc);

        var secondPut = await _putService.PutAsync(command);
        Assert.True(secondPut.IsSucc);

        var expected = firstPut.ToOption().ValueUnsafe();
        var actual = secondPut.ToOption().ValueUnsafe();

        Assert.Equal(expected, actual);
    }

    private static PutAccount.Command NewCommand(Domain.Account.Account account) =>
        new(account.Source.Name, account.Name, account.LastUpdatedAt);
}
