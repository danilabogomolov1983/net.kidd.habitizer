namespace Net.Kidd.Habitizer.Features.Account.Bulk.GetBySources;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
