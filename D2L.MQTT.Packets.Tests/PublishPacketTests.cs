using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class PublishPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( PublishPacket expectedPacket, byte[] bytes ) {

			PublishPacket packet = PacketTestHelpers.Read<PublishPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.QoS, packet.QoS, "Session present should match" );
			Assert.AreEqual( expectedPacket.Retain, packet.Retain, "Session present should match" );
			Assert.AreEqual( expectedPacket.Duplicate, packet.Duplicate, "Return code should match" );

			Assert.AreEqual( expectedPacket.Topic, packet.Topic, "Session present should match" );
			Assert.AreEqual( expectedPacket.PacketIdentifier, packet.PacketIdentifier, "Return code should match" );

			CollectionAssert.AreEqual( expectedPacket.Message, packet.Message, "Return code should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( PublishPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<PublishPacket>(
					name: "EmptyMessage",
					packet: new PublishPacket(
						topic: "topic",
						message: new byte[ 0 ],
						qos: QualityOfService.QoS0,
						packetIdentifier: 0,
						retain: false,
						duplicate: false
					),
					bytes: new byte[] {
						0x30, // Header
						7, // remaining length
						0, 5, // Topic length
						116, 111, 112, 105, 99, // Topic (topic)
					}
				);

			yield return new MqttPacketTestCase<PublishPacket>(
					name: "Message",
					packet: new PublishPacket(
						topic: "topic",
						message: Encoding.UTF8.GetBytes( "message" ),
						qos: QualityOfService.QoS0,
						packetIdentifier: 0,
						retain: false,
						duplicate: false
					),
					bytes: new byte[] {
						0x30, // Header
						14, // remaining length
						0, 5, // Topic length
						116, 111, 112, 105, 99, // Topic (topic)
						109, 101, 115, 115, 97, 103, 101 // Message (message)
					}
				);

			yield return new MqttPacketTestCase<PublishPacket>(
					name: "2KB Message",
					packet: new PublishPacket(
						topic: "topic",
						message: Encoding.UTF8.GetBytes( new string( 'A', 2048 ) ),
						qos: QualityOfService.QoS0,
						packetIdentifier: 0,
						retain: false,
						duplicate: false
					),
					bytes: Enumerable.Concat(
						new byte[] {
							0x30, // header
							135, 16, // remaining length
							0, 5, // Topic length
							116, 111, 112, 105, 99, // Topic (topic)
						},
						Enumerable.Repeat<byte>( 65, 2048 ) // Message (AAA...)
					).ToArray()
				);

			yield return new MqttPacketTestCase<PublishPacket>(
					name: "QoS1",
					packet: new PublishPacket(
						topic: "topic",
						message: new byte[ 0 ],
						qos: QualityOfService.QoS1,
						packetIdentifier: 10,
						retain: false,
						duplicate: false
					),
					bytes: new byte[] {
						0x32, // Header
						9, // remaining length
						0, 5, // Topic length
						116, 111, 112, 105, 99, // Topic (topic)
						0, 10 // Packet Identifier
					}
				);

			yield return new MqttPacketTestCase<PublishPacket>(
					name: "QoS2",
					packet: new PublishPacket(
						topic: "topic",
						message: new byte[ 0 ],
						qos: QualityOfService.QoS2,
						packetIdentifier: 23333,
						retain: false,
						duplicate: false
					),
					bytes: new byte[] {
						0x34, // Header
						9, // remaining length
						0, 5, // Topic length
						116, 111, 112, 105, 99, // Topic (topic)
						91, 37 // Packet Identifier
					}
				);

			yield return new MqttPacketTestCase<PublishPacket>(
					name: "Duplicate",
					packet: new PublishPacket(
						topic: "topic",
						message: new byte[ 0 ],
						qos: QualityOfService.QoS2,
						packetIdentifier: 23333,
						retain: false,
						duplicate: true
					),
					bytes: new byte[] {
						0x3C, // Header
						9, // remaining length
						0, 5, // Topic length
						116, 111, 112, 105, 99, // Topic (topic)
						91, 37 // Packet Identifier
					}
				);

			yield return new MqttPacketTestCase<PublishPacket>(
					name: "Retain",
					packet: new PublishPacket(
						topic: "topic",
						message: new byte[ 0 ],
						qos: QualityOfService.QoS0,
						packetIdentifier: 0,
						retain: true,
						duplicate: false
					),
					bytes: new byte[] {
						0x31, // Header
						7, // remaining length
						0, 5, // Topic length
						116, 111, 112, 105, 99, // Topic (topic)
					}
				);

			yield return new MqttPacketTestCase<PublishPacket>(
					name: "QoS2_Retain_Duplicate",
					packet: new PublishPacket(
						topic: "topic",
						message: new byte[ 0 ],
						qos: QualityOfService.QoS2,
						packetIdentifier: 34,
						retain: true,
						duplicate: true
					),
					bytes: new byte[] {
						0x3D, // Header
						9, // remaining length
						0, 5, // Topic length
						116, 111, 112, 105, 99, // Topic (topic)
						0, 34 // Packet Identifier
					}
				);

			yield return new MqttPacketTestCase<PublishPacket>(
					name: "Full",
					packet: new PublishPacket(
						topic: "topic",
						message: Encoding.UTF8.GetBytes( "message" ),
						qos: QualityOfService.QoS1,
						packetIdentifier: 34,
						retain: true,
						duplicate: true
					),
					bytes: new byte[] {
						0x3B, // Header
						16, // remaining length
						0, 5, // Topic length
						116, 111, 112, 105, 99, // Topic (topic)
						0, 34, // Packet Identifier
						109, 101, 115, 115, 97, 103, 101 // Message (message)
					}
				);
		}

		[Test]
		public void PublishPacket_InvalidTopic_ShouldThrow(
				[Values( null, "")]
				string topic
			) {

			Assert.Throws<ArgumentNullException>( () => {
				new PublishPacket(
					topic: topic,
					message: new byte[ 0 ],
					qos: QualityOfService.QoS0,
					packetIdentifier: 38383,
					retain: false,
					duplicate: false
				);
			} );
		}

		[Test]
		public void PublishPacket_NullMessage_ShouldThrow() {

			Assert.Throws<ArgumentNullException>( () => {
				new PublishPacket(
					topic: "abc",
					message: null,
					qos: QualityOfService.QoS0,
					packetIdentifier: 38383,
					retain: false,
					duplicate: false
				);
			} );
		}
	}
}
