using Moq;
using NUnit.Framework;
using System;
using System.Net.WebSockets;
using System.Threading;

namespace MessageBroker.Server.Networking.Test
{
    public class ConnectionManagerTest
    {
        private IConnectionManager _connectionManager;

        [SetUp]
        public void Setup()
        {
            _connectionManager = new ConnectionManager();
        }

        [Test]
        public void ShouldTryToCloseAddedSocketAfterRemove()
        {
            var socketMock = new Mock<WebSocket>();
            var guid = Guid.NewGuid().ToString();

            _connectionManager.AddSocket(guid, socketMock.Object);
            _connectionManager.RemoveSocket(guid);

            socketMock.Verify(s
                => s.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the ConnectionManager", CancellationToken.None)
                , Times.Once);
        }

        [Test]
        public void ShouldFindAddedSocket()
        {
            var socketMock = new Mock<WebSocket>();
            var guid = Guid.NewGuid().ToString();

            _connectionManager.AddSocket(guid, socketMock.Object);

            Assert.IsTrue(_connectionManager.GetId(socketMock.Object) == guid);
        }
    }
}