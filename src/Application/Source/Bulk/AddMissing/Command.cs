namespace Wst.Tools.PosiBridge.Application.Source.Bulk.AddMissing;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
