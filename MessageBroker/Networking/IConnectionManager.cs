using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace MessageBroker.Server.Networking
{
    public interface IConnectionManager
    {
        WebSocket GetSocketById(string id);

        ConcurrentDictionary<string, WebSocket> GetAll();

        string GetId(WebSocket socket);

        void AddSocket(WebSocket socket);

        Task RemoveSocket(string id);
    }
}