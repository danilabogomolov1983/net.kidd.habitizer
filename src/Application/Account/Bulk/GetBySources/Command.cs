namespace Wst.Tools.PosiBridge.Application.Account.Bulk.GetBySources;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
