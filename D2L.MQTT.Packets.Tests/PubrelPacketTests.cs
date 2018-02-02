using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class PubrelPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( PubrelPacket expectedPacket, byte[] bytes ) {

			PubrelPacket packet = PacketTestHelpers.Read<PubrelPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.PacketIdentifier, packet.PacketIdentifier, "Return code should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( PubrelPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<PubrelPacket>(
					name: "Default",
					packet: new PubrelPacket(
						packetIdentifier: 77
					),
					bytes: new byte[] {
						98, // Header
						2, // remaining length
						0, 77 // packet identifier
					}
				);
		}
	}
}
