using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class PingreqPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( PingreqPacket expectedPacket, byte[] bytes ) {

			PingreqPacket packet = PacketTestHelpers.Read<PingreqPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( PingreqPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<PingreqPacket>(
					name: "Default",
					packet: new PingreqPacket(),
					bytes: new byte[] {
						192, // header
						0 // remaining length
					}
				);
		}
	}
}
