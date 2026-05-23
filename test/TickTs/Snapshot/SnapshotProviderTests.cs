using FakeItEasy;
using LanguageExt;
using LanguageExt.Common;
using LanguageExt.UnsafeValueAccess;
using Microsoft.Extensions.Options;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Net.Kidd.Habitizer.TickTs.Configuration;
using Net.Kidd.Habitizer.TickTs.Http;
using Net.Kidd.Habitizer.Shared.Kernel.Functional;

namespace Net.Kidd.Habitizer.TickTs.Test.Snapshot;

public class SnapshotProviderTests
{

    public static HttpResponseMessage HttpResponseMessage_OK(TickTs.Snapshot.Response snapshot)
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new { positions = snapshot.Positions.Map(Position) },
                options: new JsonSerializerOptions(JsonSerializerDefaults.Web))
        };

        dynamic Position(TickTs.Snapshot.Position.Response position)
        {
            return new
            {
                id = position.Id,
                instrumentId = position.InstrumentId,
                netSize = position.NetSize,
                netValue = position.NetValue,
                unrealisedAverageCost = position.UnrealisedAverageCost,
                unrealisedProfit = position.UnrealisedProfit,
                unrealisedProfitPercent = position.UnrealisedProfitPercent,
                referencePrice = position.ReferencePrice != null ? new
                {
                    price = position.ReferencePrice?.Price,
                    currencyId = position.ReferencePrice?.CurrencyId,
                    exchangeId = position.ReferencePrice?.ExchangeId,
                    currencySpot = position.ReferencePrice?.CurrencySpot
                }: null

            };
        }
    }
    public static HttpResponseMessage HttpResponseMessage_BadRequest() => new(HttpStatusCode.BadRequest);
    public static HttpResponseMessage HttpResponseMessage_NullJson()
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };
    }

    public static HttpResponseMessage HttpResponseMessage_InvalidJson()
    {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{ invalid json }", Encoding.UTF8, "application/json")
        };
    }

    [Fact]
    public async Task Get_Success()
    {
        var client = A.Fake<ITickTsHttpClient>();
        var expected = Support.TickTs.SnapshotResponse();
        var expectedResponse = expected.Then(HttpResponseMessage_OK);

        A.CallTo(() => client.SendAsync(A<TickTsHttpRequestPayload>._))
            .Returns(expectedResponse.ToFinAsync());

        var provider = new TickTs.Snapshot.SnapshotProvider(client, A.Fake<IOptions<TickTsSettings>>());

        var maybeActual = await provider.GetAsync("account");
        Assert.True(maybeActual.IsSucc);
        var actual = maybeActual.ToOption().ValueUnsafe();
        Assert.NotNull(actual);

        var mappedExpected = TickTsMappers.SnapshotMapper.Map(("account", expected)); 
        Assert.Equivalent(mappedExpected, actual);
    }

    [Fact]
    public async Task Get_Failure_When_Http_Client_Fails()
    {
        var expectedError = Error.New("tick-ts request failed");
        var client = A.Fake<ITickTsHttpClient>();

        A.CallTo(() => client.SendAsync(A<TickTsHttpRequestPayload>._))
            .Returns(Task.FromResult(Fin<HttpResponseMessage>.Fail(expectedError)));

        var provider = new TickTs.Snapshot.SnapshotProvider(client, A.Fake<IOptions<TickTsSettings>>());

        var maybeActual = await provider.GetAsync("account");

        Assert.True(maybeActual.IsFail);
        maybeActual.IfFail(AssertError(expectedError));
    }

    [Fact]
    public async Task Get_Failure_When_Response_Body_Is_Empty()
    {
        var client = A.Fake<ITickTsHttpClient>();

        A.CallTo(() => client.SendAsync(A<TickTsHttpRequestPayload>._))
            .Returns(HttpResponseMessage_NullJson().ToFinAsync());

        var provider = new TickTs.Snapshot.SnapshotProvider(client, A.Fake<IOptions<TickTsSettings>>());

        var maybeActual = await provider.GetAsync("account");

        Assert.True(maybeActual.IsFail);
        maybeActual.IfFail(AssertError(Error.New("Tick-TS response body was empty.")));
    }

    [Fact]
    public async Task Get_Failure_When_Response_Body_Is_Invalid_Json()
    {
        var client = A.Fake<ITickTsHttpClient>();

        A.CallTo(() => client.SendAsync(A<TickTsHttpRequestPayload>._))
            .Returns(HttpResponseMessage_InvalidJson().ToFinAsync());

        var provider = new TickTs.Snapshot.SnapshotProvider(client, A.Fake<IOptions<TickTsSettings>>());

        var maybeActual = await provider.GetAsync("account");

        Assert.True(maybeActual.IsFail);
        maybeActual.IfFail(AssertError(Error.New("Error reading Tick-TS response body.")));
    }

    [Fact]
    public async Task Get_Success_When_Response_Contains_No_Positions()
    {
        var client = A.Fake<ITickTsHttpClient>();
        var expected = new TickTs.Snapshot.Response(ImmutableList<TickTs.Snapshot.Position.Response>.Empty);

        A.CallTo(() => client.SendAsync(A<TickTsHttpRequestPayload>._))
            .Returns(HttpResponseMessage_OK(expected).ToFinAsync());

        var provider = new TickTs.Snapshot.SnapshotProvider(client, A.Fake<IOptions<TickTsSettings>>());

        var maybeActual = await provider.GetAsync("account");

        Assert.True(maybeActual.IsSucc);
        var actual = maybeActual.ToOption().ValueUnsafe();

        var mappedExpected = TickTsMappers.SnapshotMapper.Map(("account", expected));
        Assert.Equivalent(mappedExpected, actual);
    }

    [Fact]
    public async Task Get_MultipleAccounts_Success()
    {
        var client = A.Fake<ITickTsHttpClient>();
        var firstResponse = Support.TickTs.SnapshotResponse();
        var secondResponse = Support.TickTs.SnapshotResponse();

        A.CallTo(() => client.SendAsync(A<TickTsHttpRequestPayload>.That.Matches(payload => payload.AccountId == "account-1")))
            .Returns(HttpResponseMessage_OK(firstResponse).ToFinAsync());
        A.CallTo(() => client.SendAsync(A<TickTsHttpRequestPayload>.That.Matches(payload => payload.AccountId == "account-2")))
            .Returns(HttpResponseMessage_OK(secondResponse).ToFinAsync());

        var provider = new TickTs.Snapshot.SnapshotProvider(client, A.Fake<IOptions<TickTsSettings>>());

        var maybeActual = await provider.GetAsync(["account-1", "account-2"]);

        Assert.True(maybeActual.IsSucc);
        var actual = maybeActual.ToOption().ValueUnsafe();
        Assert.NotNull(actual);

        var expected = TickTsMappers.SnapshotMapper.Map(("account-1", firstResponse)).Positions
            .Concat(TickTsMappers.SnapshotMapper.Map(("account-2", secondResponse)).Positions)
            .ToImmutableList();

        Assert.Equal(expected.Count, actual.Positions.Count);
        Assert.Equivalent(expected, actual.Positions);
    }

    [Fact]
    public async Task Get_MultipleAccounts_Failure_When_Any_Request_Fails()
    {
        var expectedError = Error.New("Errors occured on snapshot synchronization.");
        var client = A.Fake<ITickTsHttpClient>();

        A.CallTo(() => client.SendAsync(A<TickTsHttpRequestPayload>.That.Matches(payload => payload.AccountId == "account-1")))
            .Returns(HttpResponseMessage_OK(Support.TickTs.SnapshotResponse()).ToFinAsync());
        A.CallTo(() => client.SendAsync(A<TickTsHttpRequestPayload>.That.Matches(payload => payload.AccountId == "account-2")))
            .Returns(Task.FromResult(Fin<HttpResponseMessage>.Fail(expectedError)));

        var provider = new TickTs.Snapshot.SnapshotProvider(client, A.Fake<IOptions<TickTsSettings>>());

        var maybeActual = await provider.GetAsync(["account-1", "account-2"]);

        Assert.True(maybeActual.IsFail);
        maybeActual.IfFail(actualError =>
        {
            Assert.Contains("Errors occured on snapshot synchronization.", actualError.Message);
        });
    }
}
