using System.Net;
using System.Net.Sockets;
using Wst.Tools.PosiBridge.TickTs.Configuration;

namespace Wst.Tools.PosiBridge.TickTs.Http;

internal static class TickTsHttpHandlerFactory
{
    public static HttpMessageHandler Create(TickTsSettings settings)
    {
        var handler = new SocketsHttpHandler();

        handler.ConnectCallback = async (context, cancellationToken) =>
        {
            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            try
            {
                var address = IPAddress.Parse(settings.ResolvedAddress);
                await socket.ConnectAsync(address, context.DnsEndPoint.Port, cancellationToken);
                return new NetworkStream(socket, ownsSocket: true);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        };

        return handler;
    }
}
