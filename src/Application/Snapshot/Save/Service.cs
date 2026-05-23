using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using PositionPut = Net.Kidd.Habitizer.Application.Position.Put;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.Domain.ValueObjects;

namespace Net.Kidd.Habitizer.Application.Snapshot.Save;

public class Service(
    PositionPut.Service positionPutService,
    ILogger<Service> logger)
{
    public async Task<Fin<Unit>> SaveAsync(Command command)
    {
        var (source, positions) = command.Snapshot;
        
        var errors = new List<Error>();

        foreach (var position in positions)
        {
            var positionPutCommand = new PositionPut.Command(
                new SourceName(source.ToSourceName()),
                position.Account.Name,
                position.Instrument.Isin,
                position.NetSize,
                position.NetValue,
                position.UnrealisedAverageCost,
                position.UnrealisedProfit,
                position.UnrealisedProfitPercent,
                position.ReferencePrice);

            var result = await positionPutService.PutAsync(positionPutCommand);
            if (result.IsFail)
            {
                var error = result.Match(
                    _ => Error.New("Failed to save synced position"),
                    fail => fail);

                errors.Add(error);

                logger.LogError(
                    "Failed to save snapshot position for source {TargetSource}, target account {TargetAccount}, instrument {Isin}. Error: {Error}",
                    position.Account.Source.Name.Value,
                    position.Account.Name.Value,
                    position.Instrument.Isin.Value,
                    error);
            }
        }

        if (errors.Count == 0)
        {
            return Fin<Unit>.Succ(Unit.Default);
        }

        var combinedError = Error.New(
            $"Failed to save {errors.Count} synced positions. Errors: {string.Join(" | ", errors.Select(error => error.Message))}");

        return Fin<Unit>.Fail(combinedError);
    }
}
