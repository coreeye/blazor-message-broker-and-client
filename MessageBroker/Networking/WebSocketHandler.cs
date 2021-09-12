using MessageBroker.Shared;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBroker.Server.Networking
{
    public abstract class WebSocketHandler : IWebSocketHandler
    {
        private readonly IModelSerialization _modelSerialization;

        protected IConnectionManager WebSocketConnectionManager { get; set; }

        public WebSocketHandler(IConnectionManager webSocketConnectionManager, IModelSerialization modelSerialization)
        {
            WebSocketConnectionManager = webSocketConnectionManager;
            _modelSerialization = modelSerialization;
        }

        public virtual async Task OnConnected(WebSocket socket)
        {
            WebSocketConnectionManager.AddSocket(Guid.NewGuid().ToString(), socket);
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
        }

        public async Task SendMessageAsync(WebSocket socket, Response response)
        {
            if(socket.State != WebSocketState.Open)
                return;

            var buffer = _modelSerialization.SerializeModel(response);
            await socket.SendAsync(buffer,
                                   WebSocketMessageType.Text,
                                   true,
                                   CancellationToken.None);          
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

        public abstract Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}