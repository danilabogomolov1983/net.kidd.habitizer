using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;
using System.Net.Http.Json;
using System.Text.Json;
using Net.Kidd.Habitizer.Application.Snapshot;
using Net.Kidd.Habitizer.Domain.Snapshot;
using Net.Kidd.Habitizer.TickTs.Configuration;
using Net.Kidd.Habitizer.TickTs.Http;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.TickTs.Snapshot;

public sealed class SnapshotProvider(
    ITickTsHttpClient tickTsHttpClient,
    IOptions<TickTsSettings> options) : IPortfolioSnapshotProvider
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private static async Task<Fin<Response>> ReadFromJson(HttpResponseMessage that)
    {
        try
        {
            var response = await that.Content.ReadFromJsonAsync<Response>(SerializerOptions);
            return response == null ?
                    Error.New("Tick-TS response body was empty.")
                : response;
        }
        catch (Exception ex)
        {
            return Error.New("Error reading Tick-TS response body.", ex);
        }
    }

    public Task<Fin<Domain.Snapshot.Snapshot>> GetAsync(string account)
    {
        var payload = new TickTsHttpRequestPayload("portfolio", options.Value.Token, account);
        return tickTsHttpClient
            .SendAsync(payload)
            .BindAsync(ReadFromJson)
            .BindAsync(response => TickTsMappers.SnapshotMapper.Map((account, response)));
    }

    public async Task<Fin<Domain.Snapshot.Snapshot>> GetAsync(string[] accounts)
    {
        var snapshots = new List<Domain.Snapshot.Snapshot>();
        var errors = new List<Error>();
        foreach (var account in accounts)
        {
            await GetAsync(account)
                .MatchAsync(i =>
                {
                    snapshots.Add(i);
                    return Unit.SuccessAsync();
                }, err =>
                {
                    errors.Add(err);
                    return Unit.SuccessAsync();
                });
        }

        if (errors.Any())
        {
            return Fin<Domain.Snapshot.Snapshot>.Fail(Error.Many(errors.ToArray()));
        }

        var joinedPositions = snapshots.SelectMany(i => i.Positions).ToImmutableList();
        return new Domain.Snapshot.Snapshot(ESnapshotSource.TickTs, joinedPositions);
    }
}
