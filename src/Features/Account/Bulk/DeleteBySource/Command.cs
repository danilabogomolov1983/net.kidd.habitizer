namespace Net.Kidd.Habitizer.Features.Account.Bulk.DeleteBySource;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
