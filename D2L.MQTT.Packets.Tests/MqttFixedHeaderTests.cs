using System;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	internal sealed class MqttFixedHeaderTests {

		private const PacketType UnknownPacketType = 0;

		[Test]
		public void Construction_InvalidPacketType() {

			Assert.Throws<ArgumentException>( () => {
				new MqttFixedHeader( UnknownPacketType, 0x00, 0 );
			} );
		}

		[Test]
		public void ParseControlPacketByte_WhenInvalidPacketType() {

			var err = Assert.Throws<PacketFormatException>( () => {

				PacketType packetType;
				byte flags;
				MqttFixedHeader.ParseControlPacketByte(
						0x00,
						out packetType,
						out flags
					);
			} );

			Assert.AreEqual( UnknownPacketType, err.PacketType );
		}
	}
}
