using LanguageExt;

namespace Net.Kidd.Habitizer.TickTs.Http;

public interface ITickTsHttpClient
{
    Task<Fin<HttpResponseMessage>> SendAsync(TickTsHttpRequestPayload payload);
}
