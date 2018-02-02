using System.Collections.Generic;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class UnsubackPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( UnsubackPacket expectedPacket, byte[] bytes ) {

			UnsubackPacket packet = PacketTestHelpers.Read<UnsubackPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.PacketIdentifier, packet.PacketIdentifier, "Return code should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( UnsubackPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<UnsubackPacket>(
					name: "Default",
					packet: new UnsubackPacket(
						packetIdentifier: 88
					),
					bytes: new byte[] {
						176, // Header
						2, // remaining length
						0, 88 // packet identifier
					}
				);
		}
	}
}
