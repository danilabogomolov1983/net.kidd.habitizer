namespace Wst.Tools.PosiBridge.Application.Position.Bulk.DeleteBySource;

public sealed record Command(IReadOnlyList<Domain.Source.Source> Sources);
