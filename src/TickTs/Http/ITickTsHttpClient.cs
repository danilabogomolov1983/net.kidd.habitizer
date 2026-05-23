using LanguageExt;

namespace Wst.Tools.PosiBridge.TickTs.Http;

public interface ITickTsHttpClient
{
    Task<Fin<HttpResponseMessage>> SendAsync(TickTsHttpRequestPayload payload);
}
