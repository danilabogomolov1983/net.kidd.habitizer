using Net.Kidd.Habitizer.Persistence.Account;

namespace Net.Kidd.Habitizer.Persistence.Source;

public class SourceDbo
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<AccountDbo> Accounts { get; set; } = new List<AccountDbo>();
}
