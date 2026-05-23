using Wst.Tools.PosiBridge.Persistence.Account;

namespace Wst.Tools.PosiBridge.Persistence.Source;

public class SourceDbo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<AccountDbo> Accounts { get; set; } = new List<AccountDbo>();
}
