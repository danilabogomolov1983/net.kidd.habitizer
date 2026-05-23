using Net.Kidd.Habitizer.Domain.ValueObjects;
using Net.Kidd.Habitizer.Shared.Kernel;
using AccountId = Net.Kidd.Habitizer.Domain.Account.AccountId;

namespace Net.Kidd.Habitizer.Persistence.Account;

public struct Mapper : IMap<Domain.Account.Account, AccountDbo>
{
    public Domain.Account.Account Map(AccountDbo right)
    {
        ArgumentNullException.ThrowIfNull(right.Source);
        return new Domain.Account.Account(
            new AccountId(right.Id),
            DboMappers.SourceMapper.Map(right.Source),
            new AccountName(right.Name),
            right.LastUpdatedAt);
    }

    public AccountDbo Map(Domain.Account.Account left)
    {
        ArgumentNullException.ThrowIfNull(left.Source);
        return new AccountDbo
        {
            Id = left.Id.Value,
            SourceId = left.Source.Id.Value,
            Source = DboMappers.SourceMapper.Map(left.Source),
            Name = left.Name.Value,
            LastUpdatedAt = left.LastUpdatedAt
        };
    }
}
