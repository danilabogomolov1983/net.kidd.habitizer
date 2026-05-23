namespace Wst.Tools.PosiBridge.Application.Position.Bulk.GetBySource;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
