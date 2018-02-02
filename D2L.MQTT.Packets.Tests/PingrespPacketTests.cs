using System.Collections.Generic;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class PingrespPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( PingrespPacket expectedPacket, byte[] bytes ) {

			PingrespPacket packet = PacketTestHelpers.Read<PingrespPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( PingrespPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<PingrespPacket>(
					name: "Default",
					packet: new PingrespPacket(),
					bytes: new byte[] {
						208, // header
						0 // remaining length
					}
				);
		}
	}
}
