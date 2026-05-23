using Wst.Tools.PosiBridge.Domain.ValueObjects;
using Wst.Tools.PosiBridge.Shared.Kernel;
using SourceId = Wst.Tools.PosiBridge.Domain.Source.SourceId;

namespace Wst.Tools.PosiBridge.Persistence.Source;

public struct Mapper : IMap<Domain.Source.Source, SourceDbo>
{
    public Domain.Source.Source Map(SourceDbo right)
    {
        return new Domain.Source.Source(
            new SourceId(right.Id),
            new SourceName(right.Name));
    }

    public SourceDbo Map(Domain.Source.Source left)
    {
        return new SourceDbo
        {
            Id = left.Id.Value,
            Name = left.Name.Value
        };
    }
}
