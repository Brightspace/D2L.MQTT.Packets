using System.Collections.Generic;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class UnsubscribePacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( UnsubscribePacket expectedPacket, byte[] bytes ) {

			UnsubscribePacket packet = PacketTestHelpers.Read<UnsubscribePacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.PacketIdentifier, packet.PacketIdentifier, "Packet identifier should match" );
			CollectionAssert.AreEqual( expectedPacket.TopicFilters, packet.TopicFilters, "Topic filters should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( UnsubscribePacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<UnsubscribePacket>(
					name: "SingleTopic",
					packet: new UnsubscribePacket(
						packetIdentifier: 6,
						topicFilters: new[] {
							"test"
						}
					),
					bytes: new byte[] {
						162, // header
						8, // remaining length
						0, 6, // message id (6)
						0, 4, // topic length,
						116, 101, 115, 116 // topic (test)
					}
				);

			yield return new MqttPacketTestCase<UnsubscribePacket>(
					name: "Three Topics",
					packet: new UnsubscribePacket(
						packetIdentifier: 6,
						topicFilters: new[] {
							"test",
							"uest",
							"tfst"
						}
					),
					bytes: new byte[] {
						162, // header
						20, // remaining length
						0, 6, // message id (6)
						0, 4, // topic length,
						116, 101, 115, 116, // topic (test)
						0, 4, // topic length
						117, 101, 115, 116, // topic (uest)
						0, 4, // topic length
						116, 102, 115, 116 // topic (tfst)
					}
				);
		}
	}
}
