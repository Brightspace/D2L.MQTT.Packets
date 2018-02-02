using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class ConnectPacketTests {

		[TestCaseSource( nameof( ReadTestCases ) )]
		public void Read( ConnectPacket expectedPacket, byte[] bytes ) {

			ConnectPacket packet = PacketTestHelpers.Read<ConnectPacket>( bytes );
			Assert.AreEqual( expectedPacket.PacketType, packet.PacketType, "Packet type should match" );

			Assert.AreEqual( expectedPacket.ProtocolLevel, packet.ProtocolLevel, "Protocol level should match" );
			Assert.AreEqual( expectedPacket.ProtocolName, packet.ProtocolName, "Protocol name should match" );

			Assert.AreEqual( expectedPacket.ClientId, packet.ClientId, "Client id should match" );
			Assert.AreEqual( expectedPacket.CleanSession, packet.CleanSession, "Clean session should match" );
			Assert.AreEqual( expectedPacket.KeepAlive, packet.KeepAlive, "Keep alive should match" );

			Assert.AreEqual( expectedPacket.UserName, packet.UserName, "Username should match" );
			Assert.AreEqual( expectedPacket.Password, packet.Password, "Password should match" );

			Assert.AreEqual( expectedPacket.Will?.Topic, packet.Will?.Topic, "Will topic should match" );
			Assert.AreEqual( expectedPacket.Will?.QoS, packet.Will?.QoS, "Will QoS should match" );
			Assert.AreEqual( expectedPacket.Will?.Retain, packet.Will?.Retain, "Will retain should match" );

			CollectionAssert.AreEqual(
					expectedPacket.Will?.Message ?? new byte[ 0 ],
					packet.Will?.Message ?? new byte[ 0 ],
					"Will message should match"
				);
		}

		[TestCaseSource( nameof( WriteToTestCases ) )]
		public void WriteTo( ConnectPacket packet, byte[] expectedBytes ) {
			PacketTestHelpers.AssertBytes( packet, expectedBytes );
		}

		private static IEnumerable<TestCaseData> ReadTestCases => GetPacketTestCases().Prefix( "Read_" );
		private static IEnumerable<TestCaseData> WriteToTestCases => GetPacketTestCases().Prefix( "WriteTo_" );

		private static IEnumerable<TestCaseData> GetPacketTestCases() {

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "NoClientId",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: false,
						keepAlive: 30,
						userName: null,
						password: null,
						will: null
					),
					bytes: new byte[] {
						16, // header
						12, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						0, // Connect flags
						0, 30, // Keepalive
						0, 0 // Client id length
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "ClientId",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "test",
						cleanSession: false,
						keepAlive: 30,
						userName: null,
						password: null,
						will: null
					),
					bytes: new byte[] {
						16, // header
						16, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						0, // Connect flags
						0, 30, // Keepalive
						0, 4, // Client id length
						116, 101, 115, 116 // Client id
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "CleanSession",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: true,
						keepAlive: 30,
						userName: null,
						password: null,
						will: null
					),
					bytes: new byte[] {
						16, // header
						12, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						2, // Connect flags
						0, 30, // Keepalive
						0, 0 // Client id length
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "WillWithEmptyMessage",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: false,
						keepAlive: 30,
						userName: null,
						password: null,
						will: new ConnectionWill(
							topic: "topic",
							message: new byte[ 0 ],
							qos: QualityOfService.QoS0,
							retain: false
						)
					),
					bytes: new byte[] {
						16, // header
						21, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						4, // Connect flags
						0, 30, // Keepalive
						0, 0, // Client id length
						0, 5, // Will topic length
						116, 111, 112, 105, 99, // Will topic
						0, 0 // Will message length
						// Will message
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "WillWithQoS1",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: false,
						keepAlive: 30,
						userName: null,
						password: null,
						will: new ConnectionWill(
							topic: "topic",
							message: new byte[ 0 ],
							qos: QualityOfService.QoS1,
							retain: false
						)
					),
					bytes: new byte[] {
						16, // header
						21, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						12, // Connect flags
						0, 30, // Keepalive
						0, 0, // Client id length
						0, 5, // Will topic length
						116, 111, 112, 105, 99, // Will topic
						0, 0 // Will message length
						// Will message
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "WillWithQoS2",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: false,
						keepAlive: 30,
						userName: null,
						password: null,
						will: new ConnectionWill(
							topic: "topic",
							message: new byte[ 0 ],
							qos: QualityOfService.QoS2,
							retain: false
						)
					),
					bytes: new byte[] {
						16, // header
						21, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						20, // Connect flags
						0, 30, // Keepalive
						0, 0, // Client id length
						0, 5, // Will topic length
						116, 111, 112, 105, 99, // Will topic
						0, 0 // Will message length
						// Will message
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "WillWithRetain",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: false,
						keepAlive: 30,
						userName: null,
						password: null,
						will: new ConnectionWill(
							topic: "topic",
							message: new byte[ 0 ],
							qos: QualityOfService.QoS0,
							retain: true
						)
					),
					bytes: new byte[] {
						16, // header
						21, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						36, // Connect flags
						0, 30, // Keepalive
						0, 0, // Client id length
						0, 5, // Will topic length
						116, 111, 112, 105, 99, // Will topic
						0, 0 // Will message length
						// Will message
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "WillWithMessage",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: false,
						keepAlive: 30,
						userName: null,
						password: null,
						will: new ConnectionWill(
							topic: "topic",
							message: Encoding.UTF8.GetBytes( "message" ),
							qos: QualityOfService.QoS0,
							retain: true
						)
					),
					bytes: new byte[] {
						16, // header
						28, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						36, // Connect flags
						0, 30, // Keepalive
						0, 0, // Client id length
						0, 5, // Will topic length
						116, 111, 112, 105, 99, // Will topic (topic)
						0, 7, // Will message length
						109, 101, 115, 115, 97, 103, 101 // Will message (message)
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "Username",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: false,
						keepAlive: 30,
						userName: "username",
						password: null,
						will: null
					),
					bytes: new byte[] {
						16, // header
						22, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						128, // Connect flags
						0, 30, // Keepalive
						0, 0, // Client id length
						0, 8, // Username length
						117, 115, 101, 114, 110, 97, 109, 101 // Username
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "Username",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: false,
						keepAlive: 30,
						userName: "username",
						password: null,
						will: null
					),
					bytes: new byte[] {
						16, // header
						22, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						128, // Connect flags
						0, 30, // Keepalive
						0, 0, // Client id length
						0, 8, // Username length
						117, 115, 101, 114, 110, 97, 109, 101 // Username
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "Password",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: false,
						keepAlive: 30,
						userName: null,
						password: "password",
						will: null
					),
					bytes: new byte[] {
						16, // header
						22, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						64, // Connect flags
						0, 30, // Keepalive
						0, 0, // Client id length
						0, 8, // Password length
						112, 97, 115, 115, 119, 111, 114, 100 // Password
					}
				);

			yield return new MqttPacketTestCase<ConnectPacket>(
					name: "UsernameAndPassword",
					packet: new ConnectPacket(
						protocolLevel: MqttProtocolLevel.Version_3_1_1,
						protocolName: "MQTT",
						clientId: "",
						cleanSession: false,
						keepAlive: 30,
						userName: "username",
						password: "password",
						will: null
					),
					bytes: new byte[] {
						16, // header
						32, // remaining length
						0, 4, // Protocol id length
						77, 81, 84, 84, // Protocol id
						4, // Protocol version
						192, // Connect flags
						0, 30, // Keepalive
						0, 0, // Client id length
						0, 8, // Username length
						117, 115, 101, 114, 110, 97, 109, 101, // Username
						0, 8, // Password length
						112, 97, 115, 115, 119, 111, 114, 100 // Password
					}
				);
		}

		[Test]
		public void ConnectPacket_InvalidProtocolName_ShouldThrow(
				[Values( null, "")]
				string protocolName
			) {

			Assert.Throws<ArgumentNullException>( () => {
				new ConnectPacket(
					protocolLevel: MqttProtocolLevel.Version_3_1_1,
					protocolName: protocolName,
					clientId: "",
					cleanSession: false,
					keepAlive: 30,
					userName: null,
					password: null,
					will: new ConnectionWill(
						topic: "topic",
						message: new byte[ 0 ],
						qos: QualityOfService.QoS0,
						retain: true
					)
				);
			} );
		}

		[Test]
		public void ConnectPacket_NullClientId_ShouldThrow() {

			Assert.Throws<ArgumentNullException>( () => {
				new ConnectPacket(
					protocolLevel: MqttProtocolLevel.Version_3_1_1,
					protocolName: "MQTT",
					clientId: null,
					cleanSession: false,
					keepAlive: 30,
					userName: null,
					password: null,
					will: new ConnectionWill(
						topic: "topic",
						message: new byte[ 0 ],
						qos: QualityOfService.QoS0,
						retain: true
					)
				);
			} );
		}
	}
}
