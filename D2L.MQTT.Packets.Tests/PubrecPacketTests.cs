using System.Collections.Generic;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class PubrecPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( PubrecPacket expectedPacket, byte[] bytes ) {

			PubrecPacket packet = PacketTestHelpers.Read<PubrecPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.PacketIdentifier, packet.PacketIdentifier, "Return code should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( PubrecPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<PubrecPacket>(
					name: "Default",
					packet: new PubrecPacket(
						packetIdentifier: 66
					),
					bytes: new byte[] {
						80, // Header
						2, // remaining length
						0, 66 // packet identifier
					}
				);
		}
	}
}
