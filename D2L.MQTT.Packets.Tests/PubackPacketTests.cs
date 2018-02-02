using System.Collections.Generic;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class PubackPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( PubackPacket expectedPacket, byte[] bytes ) {

			PubackPacket packet = PacketTestHelpers.Read<PubackPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.PacketIdentifier, packet.PacketIdentifier, "Return code should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( PubackPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<PubackPacket>(
					name: "Default",
					packet: new PubackPacket(
						packetIdentifier: 44
					),
					bytes: new byte[] {
						64, // Header
						2, // remaining length
						0, 44 // packet identifier
					}
				);
		}
	}
}
