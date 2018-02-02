using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class DisconnectPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( DisconnectPacket expectedPacket, byte[] bytes ) {

			DisconnectPacket packet = PacketTestHelpers.Read<DisconnectPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( DisconnectPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<DisconnectPacket>(
					name: "Default",
					packet: new DisconnectPacket(),
					bytes: new byte[] {
						224, // header
						0 // remaining length
					}
				);
		}
	}
}
