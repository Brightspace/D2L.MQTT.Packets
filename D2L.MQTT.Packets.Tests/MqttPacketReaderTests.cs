using System.IO;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	internal sealed class MqttPacketReaderTests {

		[Test]
		public void ReadPacket_WhenNoBytes() {

			using( MemoryStream ms = new MemoryStream() ) {
				MqttPacket packet = MqttPacketReader.ReadPacket( ms );
				Assert.IsNull( packet );
			}
		}

		[Test]
		public void ReadPacket_WhenOnlySingleByte() {

			using( MemoryStream ms = new MemoryStream(

					// incomplete CONNECT packet
					new byte[] { 0x10 }
				) ) {

				PacketFormatException err = Assert.Throws<PacketFormatException>( () => {
					MqttPacketReader.ReadPacket( ms );
				} );

				Assert.AreEqual( PacketType.Connect, err.PacketType );
			}
		}

		[Test]
		public void ReadFixedHeader_WhenNoBytes() {

			using( MemoryStream ms = new MemoryStream() ) {
				MqttFixedHeader header = MqttPacketReader.ReadFixedHeader( ms );
				Assert.IsNull( header );
			}
		}

		[Test]
		public void ReadFixedHeader_WhenOnlySingleByte() {

			using( MemoryStream ms = new MemoryStream(

					// incomplete CONNECT packet
					new byte[] { 0x10 }
				) ) {

				PacketFormatException err = Assert.Throws<PacketFormatException>( () => {
					MqttPacketReader.ReadFixedHeader( ms );
				} );

				Assert.AreEqual( PacketType.Connect, err.PacketType );
			}
		}
	}
}
