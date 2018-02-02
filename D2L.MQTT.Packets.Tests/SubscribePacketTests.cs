using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using NUnit.Framework;
using System;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class SubscribePacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( SubscribePacket expectedPacket, byte[] bytes ) {

			SubscribePacket packet = PacketTestHelpers.Read<SubscribePacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.PacketIdentifier, packet.PacketIdentifier, "Packet identifier should match" );

			IEnumerable<Tuple<string, QualityOfService>> expectedSubscriptions = GetSubscriptionTuples( expectedPacket );
			IEnumerable<Tuple<string, QualityOfService>> actualSubscriptions = GetSubscriptionTuples( packet );
			CollectionAssert.AreEqual( expectedSubscriptions, actualSubscriptions, "Subscriptions should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( SubscribePacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<SubscribePacket>(
					name: "SingleTopic",
					packet: new SubscribePacket(
						packetIdentifier: 6,
						subscriptions: new[] {
							new Subscription( "test", QualityOfService.QoS0 )
						}
					),
					bytes: new byte[] {
						130, 9, // Header (subscribeqos=1length=9)
						0, 6, // Message id (6)
						0, 4, // Topic length,
						116, 101, 115, 116, // Topic (test)
						0 // Qos (0)
					}
				);

			yield return new MqttPacketTestCase<SubscribePacket>(
					name: "Three Topics",
					packet: new SubscribePacket(
						packetIdentifier: 6,
						subscriptions: new[] {
							new Subscription( "test", QualityOfService.QoS0 ),
							new Subscription( "uest", QualityOfService.QoS1 ),
							new Subscription( "tfst", QualityOfService.QoS2 )
						}
					),
					bytes: new byte[] {
						130, 23, // Header (publishqos=1length=9)
						0, 6, // Message id (6)
						0, 4, // Topic length,
						116, 101, 115, 116, // Topic (test)
						0, // Qos (0)
						0, 4, // Topic length
						117, 101, 115, 116, // Topic (uest)
						1, // Qos (1)
						0, 4, // Topic length
						116, 102, 115, 116, // Topic (tfst)
						2 // Qos (2)
					}
				);
		}

		private static IEnumerable<Tuple<string, QualityOfService>> GetSubscriptionTuples(
				SubscribePacket packet
			) {

			IEnumerable<Tuple<string, QualityOfService>> tuples = packet.Subscriptions
				.Select( s => new Tuple<string, QualityOfService>( s.TopicFilter, s.QoS ) );

			return tuples;
		}
	}
}
