using MessageBroker.Shared;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBroker.Networking
{
    public abstract class WebSocketHandler
    {
        protected ConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(ConnectionManager webSocketConnectionManager)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(socket);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, Response response)
        {
            if(socket.State != WebSocketState.Open)
                return;

            var buffer = SerializeResponse(response);
            await socket.SendAsync(buffer,
                                   WebSocketMessageType.Text,
                                   true,
                                   cancellationToken: CancellationToken.None);          
        }

        public async Task SendMessageAsync(string socketId, Response response)
        {
            await SendMessageAsync(WebSocketConnectionManager.GetSocketById(socketId), response);
        }

        public async Task SendMessageToAllAsync(Response response)
        {
            foreach(var pair in WebSocketConnectionManager.GetAll())
            {
                if(pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, response);
            }
        }

        private static ArraySegment<byte> SerializeResponse(Response response)
        {
            var data = JsonConvert.SerializeObject(response, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            return buffer;
        }

        //TODO - decide if exposing the message string is better than exposing the result and buffer
        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}