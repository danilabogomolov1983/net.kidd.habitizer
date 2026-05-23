using LanguageExt;
using LanguageExt.Common;
using System.Net.Http.Json;
using System.Text.Json;

namespace Wst.Tools.PosiBridge.TickTs.Http;

public class TickTsHttpClient(HttpClient httpClient):ITickTsHttpClient
{
    private const string Version = "v78";
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    public async Task<Fin<HttpResponseMessage>> SendAsync(TickTsHttpRequestPayload payload)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"api/rest/{Version}")
        {
            Content = JsonContent.Create(payload, options: SerializerOptions)
        };

        try
        {
            var response = await httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }
            return Error.New($"Tick-TS request failed with status code. Account: {payload.AccountId}. Status code: {(int)response.StatusCode}.");
        }
        catch (Exception e)
        {
            return Error.New($"Tick-TS request threw an exception. Account: {payload.AccountId}.", e);
        }
    }
}
