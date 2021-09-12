using MessageBroker.Server.Networking;
using MessageBroker.Shared;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MessageBroker.Server.Test
{
    public class Tests
    {
        private Mock<IConnectionManager> _connectionManager;
        private Mock<IModelSerialization> _serialization;
        private BrokerServer _brokerServer;
        private WebSocketReceiveResult _webSocketReceiveResult;
        private Mock<WebSocket> _socketMock;

        [SetUp]
        public void Setup()
        {
            _connectionManager = new Mock<IConnectionManager>();
            _serialization = new Mock<IModelSerialization>();

            _brokerServer = new BrokerServer(_connectionManager.Object, _serialization.Object);

            _webSocketReceiveResult = new WebSocketReceiveResult(0, WebSocketMessageType.Text, true);

            _socketMock = new Mock<WebSocket>();
            _socketMock.SetupGet(x => x.State).Returns(WebSocketState.Open);
        }

        [Test]
        public async Task ShouldReturnTopicAfterCreateTopicCall()
        {
            var request = new CreateTopicRequest("TestTopic");
            _serialization.Setup(s => s.DeserializeModel<Request>(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns(request);

            await _brokerServer.ReceiveAsync(_socketMock.Object, _webSocketReceiveResult, It.IsAny<byte[]>());

            _serialization.Verify(s => s.SerializeModel(It.Is<Response>(r => r.GetType() == typeof(TopicCreatedResponse))));
        }

        [Test]
        public async Task ShouldReturnSubscribeResponseAfterSubscribeRequest()
        {
            var request = new CreateTopicRequest("TestTopic");
            _serialization.Setup(s => s.DeserializeModel<Request>(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns(request);

            await _brokerServer.ReceiveAsync(_socketMock.Object, _webSocketReceiveResult, It.IsAny<byte[]>());

            var request2 = new SubscribeRequest("TestTopic");
            _serialization.Setup(s => s.DeserializeModel<Request>(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns(request2);

            await _brokerServer.ReceiveAsync(_socketMock.Object, _webSocketReceiveResult, It.IsAny<byte[]>());

            _serialization.Verify(s => s.SerializeModel(It.Is<Response>(r => r.GetType() == typeof(SubscribedResponse))));
        }

        [Test]
        public async Task ShouldReturnListTopicsResponseAfterListTopicsRequest()
        {
            var request = new ListTopicsRequest();
            _serialization.Setup(s => s.DeserializeModel<Request>(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Returns(request);

            await _brokerServer.ReceiveAsync(_socketMock.Object, _webSocketReceiveResult, It.IsAny<byte[]>());

            _serialization.Verify(s => s.SerializeModel(It.Is<Response>(r => r.GetType() == typeof(ListTopicsResponse))));
        }
    }
}