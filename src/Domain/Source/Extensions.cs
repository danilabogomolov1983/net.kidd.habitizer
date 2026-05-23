using Wst.Tools.PosiBridge.Domain.ValueObjects;

namespace Wst.Tools.PosiBridge.Domain.Source;

public static class Extensions
{
    extension(Source source)
    {
        public Source WithName(string name) =>
            source with { Name = new SourceName(name) };

        public Source WithName(SourceName name) =>
            source with { Name = name };
    }
}
