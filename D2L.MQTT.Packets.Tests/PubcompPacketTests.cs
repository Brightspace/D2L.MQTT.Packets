using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class PubcompPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( PubcompPacket expectedPacket, byte[] bytes ) {

			PubcompPacket packet = PacketTestHelpers.Read<PubcompPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.PacketIdentifier, packet.PacketIdentifier, "Return code should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( PubcompPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<PubcompPacket>(
					name: "Default",
					packet: new PubcompPacket(
						packetIdentifier: 55
					),
					bytes: new byte[] {
						112, // Header
						2, // remaining length
						0, 55 // packet identifier
					}
				);
		}
	}
}
