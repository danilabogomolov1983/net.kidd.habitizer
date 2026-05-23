using LanguageExt;

namespace Net.Kidd.Habitizer.Application.Snapshot;

public interface IPortfolioSnapshotProvider
{
    Task<Fin<Domain.Snapshot.Snapshot>> GetAsync(string account);
    
    Task<Fin<Domain.Snapshot.Snapshot>> GetAsync(string[] accounts);
    
    
}
