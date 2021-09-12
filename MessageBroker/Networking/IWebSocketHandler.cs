using MessageBroker.Shared;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MessageBroker.Server.Networking
{
    public interface IWebSocketHandler
    {
        Task OnConnected(WebSocket socket);

        Task OnDisconnected(WebSocket socket);

        Task SendMessageAsync(WebSocket socket, Response response);

        Task SendMessageAsync(string socketId, Response response);

        Task SendMessageToAllAsync(Response response);

        Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}