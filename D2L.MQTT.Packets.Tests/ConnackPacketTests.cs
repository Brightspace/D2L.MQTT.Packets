using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class ConnackPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( ConnackPacket expectedPacket, byte[] bytes ) {

			ConnackPacket packet = PacketTestHelpers.Read<ConnackPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.SessionPresent, packet.SessionPresent, "Session present should match" );
			Assert.AreEqual( expectedPacket.ReturnCode, packet.ReturnCode, "Return code should match" );
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( ConnackPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {
			yield return new MqttPacketTestCase<ConnackPacket>(
					name: "CleanAndSuccessful",
					packet: new ConnackPacket(
						sessionPresent: false,
						returnCode: ConnectReturnCode.ConnectionAccepted
					),
					bytes: new byte[] {
						0x20, // header
						0x02, // remaining length
						0x00, // flags
						0x00 // return code
					}
				);

			yield return new MqttPacketTestCase<ConnackPacket>(
					name: "SessionPresent",
					packet: new ConnackPacket(
						sessionPresent: true,
						returnCode: ConnectReturnCode.ConnectionAccepted
					),
					bytes: new byte[] {
						0x20, // header
						0x02, // remaining length
						0x01, // flags
						0x00 // return code
					}
				);

			yield return new MqttPacketTestCase<ConnackPacket>(
					name: "UnacceptableProtocolVersion",
					packet: new ConnackPacket(
						sessionPresent: false,
						returnCode: ConnectReturnCode.UnacceptableProtocolVersion
					),
					bytes: new byte[] {
						0x20, // header
						0x02, // remaining length
						0x00, // flags
						0x01 // return code
					}
				);


			yield return new MqttPacketTestCase<ConnackPacket>(
					name: "IdentifierRejected",
					packet: new ConnackPacket(
						sessionPresent: false,
						returnCode: ConnectReturnCode.IdentifierRejected
					),
					bytes: new byte[] {
						0x20, // header
						0x02, // remaining length
						0x00, // flags
						0x02 // return code
					}
				);

			yield return new MqttPacketTestCase<ConnackPacket>(
					name: "ServerUnavailable",
					packet: new ConnackPacket(
						sessionPresent: false,
						returnCode: ConnectReturnCode.ServerUnavailable
					),
					bytes: new byte[] {
						0x20, // header
						0x02, // remaining length
						0x00, // flags
						0x03 // return code
					}
				);

			yield return new MqttPacketTestCase<ConnackPacket>(
					name: "NotAuthorized",
					packet: new ConnackPacket(
						sessionPresent: false,
						returnCode: ConnectReturnCode.BadUserNameOrPassword
					),
					bytes: new byte[] {
						0x20, // header
						0x02, // remaining length
						0x00, // flags
						0x04 // return code
					}
				);

			yield return new MqttPacketTestCase<ConnackPacket>(
					name: "NotAuthorized",
					packet: new ConnackPacket(
						sessionPresent: false,
						returnCode: ConnectReturnCode.NotAuthorized
					),
					bytes: new byte[] {
						0x20, // header
						0x02, // remaining length
						0x00, // flags
						0x05 // return code
					}
				);
		}
	}
}
