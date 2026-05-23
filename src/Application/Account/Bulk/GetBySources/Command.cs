namespace Net.Kidd.Habitizer.Application.Account.Bulk.GetBySources;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
