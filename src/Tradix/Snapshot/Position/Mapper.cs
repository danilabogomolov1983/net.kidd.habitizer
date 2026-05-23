using Wst.Tools.PosiBridge.Domain.Account;
using Wst.Tools.PosiBridge.Domain.Position.ReferencePrice;
using Wst.Tools.PosiBridge.Domain.Source;
using Wst.Tools.PosiBridge.Domain.ValueObjects;
using Wst.Tools.PosiBridge.Shared.Kernel;

namespace Wst.Tools.PosiBridge.Tradix.Snapshot.Position;

public struct Mapper : IOnewayMap<Domain.Position.Position, (Source, PositionDbo)>
{
    public Domain.Position.Position Map((Source, PositionDbo) right)
    {
        var (source, dbo) = right;
        var instrument = dbo.Isin != null
            ? new Domain.Instrument.Instrument(Domain.Instrument.InstrumentId.Empty(), new Isin(dbo.Isin!))
            : Domain.Instrument.Instrument.Empty();

        var referencePrice = dbo.Price != null || dbo.Exchange != null
            ? new ReferencePrice(
                dbo.Price,
                
                null,
                dbo.Exchange,
                null)
            : null;


        var account = dbo.Account != null
            ? new Account(AccountId.Empty(), source, new AccountName(dbo.Account))
            : Account.Empty();
        return new Domain.Position.Position(
            account,
            instrument,
            dbo.NetSize,
            dbo.NetValue,
            null,
            dbo.UnrealisedProfit,
            null,
            referencePrice);
    }
}
