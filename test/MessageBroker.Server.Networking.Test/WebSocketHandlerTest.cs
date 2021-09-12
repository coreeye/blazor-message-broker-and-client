using MessageBroker.Shared;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBroker.Server.Networking.Test
{
    public class WebSocketHandlerTest
    {
        private Mock<IConnectionManager> _connectionManager;
        private WebSocketHandler _webSocketHandler;

        [SetUp]
        public void Setup()
        {
            _connectionManager = new Mock<IConnectionManager>();
            _webSocketHandler = new TestWebSocketHandlerDelegate(_connectionManager.Object);
        }

        [Test]
        public async Task ShouldSendMessageToAllSockets()
        {
            var response = new Mock<Response>();
            response.Setup(c => c.SerializeModel()).Returns(new ArraySegment<byte>());

            var socketInfos = new ConcurrentDictionary<string, WebSocket>();

            var socket1Mock = new Mock<WebSocket>();
            socket1Mock.SetupGet(x => x.State).Returns(WebSocketState.Open);

            var socket2Mock = new Mock<WebSocket>();
            socket2Mock.SetupGet(x => x.State).Returns(WebSocketState.Open);

            socketInfos.TryAdd("1", socket1Mock.Object);
            socketInfos.TryAdd("2", socket2Mock.Object);

            _connectionManager.Setup(c => c.GetAll()).Returns(socketInfos);

            await _webSocketHandler.SendMessageToAllAsync(response.Object);

            socket1Mock.Verify(s
                => s.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None)
                , Times.Once);

            socket2Mock.Verify(s
                => s.SendAsync(It.IsAny<ArraySegment<byte>>(), WebSocketMessageType.Text, true, CancellationToken.None)
                , Times.Once);
        }
    }

    class TestWebSocketHandlerDelegate : WebSocketHandler
    {
        public TestWebSocketHandlerDelegate(IConnectionManager webSocketConnectionManager)
            : base(webSocketConnectionManager)
        {

        }

        public override Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            return default;
        }
    }
}
