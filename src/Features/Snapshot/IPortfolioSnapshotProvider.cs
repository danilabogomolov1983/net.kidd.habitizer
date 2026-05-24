using LanguageExt;

namespace Net.Kidd.Habitizer.Features.Snapshot;

public interface IPortfolioSnapshotProvider
{
    Task<Fin<Domain.Snapshot.Snapshot>> GetAsync(string account);
    
    Task<Fin<Domain.Snapshot.Snapshot>> GetAsync(string[] accounts);
    
    
}
