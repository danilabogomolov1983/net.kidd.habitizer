using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Features.Position.Get;

public sealed record Command(SourceName SourceName, AccountName AccountName, Isin Isin);

