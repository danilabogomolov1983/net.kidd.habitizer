namespace Net.Kidd.Habitizer.Application.Account.Bulk.DeleteBySource;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
