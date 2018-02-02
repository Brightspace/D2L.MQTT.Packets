using System.Collections.Generic;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class SubackPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( SubackPacket expectedPacket, byte[] bytes ) {

			SubackPacket packet = PacketTestHelpers.Read<SubackPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.PacketIdentifier, packet.PacketIdentifier, "Packet identifier should match" );
			CollectionAssert.AreEqual( expectedPacket.ReturnCodes, packet.ReturnCodes, "Return codes should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( SubackPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<SubackPacket>(
					name: "ReturnCodeQoS0",
					packet: new SubackPacket(
						packetIdentifier: 3,
						returnCodes: new[] {
							SubscribeReturnCode.QoS0
						}
					),
					bytes: new byte[] {
						144, // header
						3, // remaining length
						0, 3, // message id (6)
						0, // return code (qos0)
					}
				);

			yield return new MqttPacketTestCase<SubackPacket>(
					name: "ReturnCodeQoS1",
					packet: new SubackPacket(
						packetIdentifier: 3,
						returnCodes: new[] {
							SubscribeReturnCode.QoS1
						}
					),
					bytes: new byte[] {
						144, // header
						3, // remaining length
						0, 3, // message id (6)
						1 // return code (qos0)
					}
				);

			yield return new MqttPacketTestCase<SubackPacket>(
					name: "ReturnCodeQoS2",
					packet: new SubackPacket(
						packetIdentifier: 3,
						returnCodes: new[] {
							SubscribeReturnCode.QoS2
						}
					),
					bytes: new byte[] {
						144, // header
						3, // remaining length
						0, 3, // message id (6)
						2 // return code (qos0)
					}
				);

			yield return new MqttPacketTestCase<SubackPacket>(
					name: "ReturnCodeFailure",
					packet: new SubackPacket(
						packetIdentifier: 3,
						returnCodes: new[] {
							SubscribeReturnCode.Failure
						}
					),
					bytes: new byte[] {
						144, // header
						3, // remaining length
						0, 3, // message id (6)
						128 // return code (qos0)
					}
				);

			yield return new MqttPacketTestCase<SubackPacket>(
					name: "ManyReturnCodes",
					packet: new SubackPacket(
						packetIdentifier: 99,
						returnCodes: new[] {
							SubscribeReturnCode.QoS0,
							SubscribeReturnCode.QoS1,
							SubscribeReturnCode.QoS2,
							SubscribeReturnCode.Failure
						}
					),
					bytes: new byte[] {
						144, // header
						6, // remaining length
						0, 99, // message id (6)
						0, 1, 2, 128 // return codes (qos0, qos1, qos2, failure)
					}
				);
		}

	}
}
