using MessageBroker.Server.Networking;
using Moq;
using NUnit.Framework;

namespace MessageBroker.Server.Test
{
    public class Tests
    {
        private Mock<IConnectionManager> _connectionManager;
        private Mock<IModelSerialization> _serialization;
        private BrokerServer _brokerServer;

        [SetUp]
        public void Setup()
        {
            _connectionManager = new Mock<IConnectionManager>();
            _serialization = new Mock<IModelSerialization>();

            _brokerServer = new BrokerServer(_connectionManager.Object, _serialization.Object);
        }

        [Test]
        public void ShouldCreateTopicAfterCreateTopicCall()
        {
        }
    }
}