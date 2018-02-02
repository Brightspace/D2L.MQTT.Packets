using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	internal sealed class IAsyncMqttPacketVisitorTests {

		private static readonly CancellationToken TestCancellationToken = new CancellationToken( false );
		private static readonly Task TestTask = Task.FromResult( true );

		private Mock<IAsyncMqttPacketVisitor> m_visitor;

		[SetUp]
		public void SetUp() {
			m_visitor = new Mock<IAsyncMqttPacketVisitor>( MockBehavior.Strict );
		}

		[TearDown]
		public void TearDown() {
			m_visitor.VerifyAll();
		}

		private void TestPacket( MqttPacket packet ) {

			var result = packet.VisitAsync( m_visitor.Object, TestCancellationToken );
			Assert.AreSame( TestTask, result );
		}

		[Test]
		public void Connect() {

			ConnectPacket packet = new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "abc",
						cleanSession: false,
						keepAlive: 30,
						userName: null,
						password: null,
						will: null
					);

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Connack() {

			ConnackPacket packet = new ConnackPacket(
					sessionPresent: false,
					returnCode: ConnectReturnCode.ConnectionAccepted
				);

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void PublishPacket() {

			PublishPacket packet = new PublishPacket(
					topic: "topic",
					message: new byte[ 0 ],
					qos: QualityOfService.QoS0,
					packetIdentifier: 0,
					retain: false,
					duplicate: false
				);

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Puback() {

			PubackPacket packet = new PubackPacket( 24646 );

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Pubrec() {

			PubrecPacket packet = new PubrecPacket( 24646 );

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Pubrel() {

			PubrelPacket packet = new PubrelPacket( 24646 );

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Pubcomp() {

			PubcompPacket packet = new PubcompPacket( 24646 );

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Subscribe() {

			SubscribePacket packet = new SubscribePacket(
					packetIdentifier: 6,
					subscriptions: new[] {
						new Subscription( "test", QualityOfService.QoS0 )
					}
				);

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Suback() {

			SubackPacket packet = new SubackPacket(
					packetIdentifier: 3,
					returnCodes: new[] {
						SubscribeReturnCode.QoS0
					}
				);

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Unsubscribe() {

			UnsubscribePacket packet = new UnsubscribePacket(
					packetIdentifier: 6,
					topicFilters: new[] {
						"test"
					}
				);

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Unsuback() {

			UnsubackPacket packet = new UnsubackPacket(
					packetIdentifier: 6
				);

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Pingreq() {

			PingreqPacket packet = new PingreqPacket();

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Pingresp() {

			PingrespPacket packet = new PingrespPacket();

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}

		[Test]
		public void Disconnect() {

			DisconnectPacket packet = new DisconnectPacket();

			m_visitor
				.Setup( v => v.VisitAsync( packet, TestCancellationToken ) )
				.Returns( TestTask );

			TestPacket( packet );
		}
	}
}
