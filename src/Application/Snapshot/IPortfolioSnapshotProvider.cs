using LanguageExt;

namespace Wst.Tools.PosiBridge.Application.Snapshot;

public interface IPortfolioSnapshotProvider
{
    Task<Fin<Domain.Snapshot.Snapshot>> GetAsync(string account);
    
    Task<Fin<Domain.Snapshot.Snapshot>> GetAsync(string[] accounts);
    
    
}
