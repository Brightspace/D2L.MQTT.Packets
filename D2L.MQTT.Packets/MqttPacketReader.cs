﻿using System.IO;

namespace D2L.MQTT.Packets {

	public static class MqttPacketReader {

		/// <summary>
		/// Reads a MQTT packet.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <exception cref="PacketFormatException">Thrown if the stream does not define a valid MQTT packet.</exception>
		/// <returns>Returns the MQTT packet if the stream is not empty; otherwise, <c>null</c>.</returns>
		public static MqttPacket ReadPacket( Stream stream ) {

			MqttFixedHeader header = ReadFixedHeader( stream );
			if( header == null ) {
				return null;
			}

			MqttPacket packet = ReadRemainingPacket( header, stream );
			return packet;
		}

		/// <summary>
		/// Reads the fixed header of a MQTT packet.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <exception cref="PacketFormatException">Thrown if the stream does not define a valid fixed header.</exception>
		/// <returns>Returns the MQTT packet's fixed header if the stream is not empty; otherwise, <c>null</c>.</returns>
		public static MqttFixedHeader ReadFixedHeader( Stream stream ) {

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

		/// <summary>
		/// Reads the remaining MQTT packet given the fixed header.
		/// </summary>
		/// <param name="fixedHeader">The fixed header.</param>
		/// <param name="stream">The stream.</param>
		/// <exception cref="PacketFormatException">Thrown if the stream does not define a valid MQTT packet for the given fixed header.</exception>
		/// <returns>Returns the remaining MQTT packet for the given fixed header.</returns>
		public static MqttPacket ReadRemainingPacket(
				MqttFixedHeader fixedHeader,
				Stream stream
			) {

			switch( fixedHeader.PacketType ) {

				case PacketType.Connect:
					return ConnectPacket.Read( fixedHeader, stream );

				case PacketType.Connack:
					return ConnackPacket.Read( fixedHeader, stream );

				case PacketType.Publish:
					return PublishPacket.Read( fixedHeader, stream );

				case PacketType.Puback:
					return PubackPacket.Read( fixedHeader, stream );

				case PacketType.Pubrec:
					return PubrecPacket.Read( fixedHeader, stream );

				case PacketType.Pubrel:
					return PubrelPacket.Read( fixedHeader, stream );

				case PacketType.Pubcomp:
					return PubcompPacket.Read( fixedHeader, stream );

				case PacketType.Subscribe:
					return SubscribePacket.Read( fixedHeader, stream );

				case PacketType.Suback:
					return SubackPacket.Read( fixedHeader, stream );

				case PacketType.Unsubscribe:
					return UnsubscribePacket.Read( fixedHeader, stream );

				case PacketType.Unsuback:
					return UnsubackPacket.Read( fixedHeader, stream );

				case PacketType.Pingreq:
					return PingreqPacket.Read( fixedHeader, stream );

				case PacketType.Pingresp:
					return PingrespPacket.Read( fixedHeader, stream );

				case PacketType.Disconnect:
					return DisconnectPacket.Read( fixedHeader, stream );

				default:
					return null;
			}
		}
	}
}
