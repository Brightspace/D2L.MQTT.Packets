using System.IO;

namespace D2L.MQTT.Packets {

	public static class MqttPacketReader {

		public static MqttPacket ReadPacket( Stream stream ) {

			MqttFixedHeader header = ReadFixedHeader( stream );
			if( header == null ) {
				return null;
			}

			MqttPacket packet = ReadPacket( header, stream );
			return packet;
		}

		private static MqttFixedHeader ReadFixedHeader( Stream stream ) {

			byte[] headerBuffer = new byte[ 1 ];

			int count = stream.Read( headerBuffer, 0, 1 );
			if( count == 0 ) {
				return null;
			}

			byte headerByte = headerBuffer[ 0 ];
			PacketType packetType = (PacketType)( ( headerByte >> 4 ) & 0x0F );
			byte flags = (byte)( headerByte & 0x0F );

			int? length = stream.ReadVariableLength();
			if( !length.HasValue ) {
				throw new PacketFormatException( packetType, "Invalid remaining length" );
			}

			return new MqttFixedHeader( packetType, flags, length.Value );
		}

		private static MqttPacket ReadPacket(
				MqttFixedHeader header,
				Stream stream
			) {

			switch( header.PacketType ) {

				case PacketType.Connect:
					return ConnectPacket.Read( header, stream );

				case PacketType.Connack:
					return ConnackPacket.Read( header, stream );

				case PacketType.Publish:
					return PublishPacket.Read( header, stream );

				case PacketType.Puback:
					return PubackPacket.Read( header, stream );

				case PacketType.Pubrec:
					return PubrecPacket.Read( header, stream );

				case PacketType.Pubrel:
					return PubrelPacket.Read( header, stream );

				case PacketType.Pubcomp:
					return PubcompPacket.Read( header, stream );

				case PacketType.Subscribe:
					return SubscribePacket.Read( header, stream );

				case PacketType.Suback:
					return SubackPacket.Read( header, stream );

				case PacketType.Unsubscribe:
					return UnsubscribePacket.Read( header, stream );

				case PacketType.Unsuback:
					return UnsubackPacket.Read( header, stream );

				case PacketType.Pingreq:
					return PingreqPacket.Read( header, stream );

				case PacketType.Pingresp:
					return PingrespPacket.Read( header, stream );

				case PacketType.Disconnect:
					return DisconnectPacket.Read( header, stream );

				default:
					return null;
			}
		}
	}
}
