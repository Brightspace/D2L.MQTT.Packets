using System;
using System.IO;

namespace D2L.MQTT.Packets {

	internal static class MqttStreamExtensions {

		public const int MaxPacketLength = 268435455;

		public static void WriteMqttFixedHeader(
				this Stream stream,
				PacketType type,
				byte flags,
				int remainingLength
			) {

			if( remainingLength > MaxPacketLength ) {
				throw new ArgumentOutOfRangeException( "remainingLength", "Packet too long" );
			}

			byte byte1 = (byte)(
				( (byte)type << 4 )
				| ( flags & 0x0F )
			);

			stream.WriteByte( byte1 );
			stream.WriteRemainingLength( remainingLength );
		}

		internal static void WriteRemainingLength(
				this Stream stream,
				int remainingLength
			) {

			do {
				byte encodedByte = (byte)( remainingLength % 128 );
				remainingLength = remainingLength / 128;

				if( remainingLength > 0 ) {
					encodedByte |= 128;
				}

				stream.WriteByte( encodedByte );

			} while( remainingLength > 0 );
		}

		public static void WriteMqttString(
				this Stream stream,
				byte[] value
			) {

			if( value == null ) {
				throw new ArgumentNullException( nameof( value ) );
			}

			stream.WriteBigEndianUInt16( value.Length );
			stream.Write( value, 0, value.Length );
		}

		public static void WriteMqttStringIfNotNull(
				this Stream stream,
				byte[] value
			) {

			if( value == null ) {
				return;
			}

			stream.WriteBigEndianUInt16( value.Length );
			stream.Write( value, 0, value.Length );
		}

		private static void WriteBigEndianUInt16(
				this Stream stream,
				int value
			) {

			ushort shortValue = Convert.ToUInt16( value );
			stream.WriteBigEndian( shortValue );
		}

		public static void WriteBigEndian(
				this Stream stream,
				ushort value
			) {

			byte[] bytes = BitConverter.GetBytes( value );

			if( BitConverter.IsLittleEndian ) {
				Array.Reverse( bytes );
			}

			stream.Write( bytes, 0, bytes.Length );
		}

		public static byte ReadByteOrThrow(
				this Stream stream,
				PacketType packetType,
				string exceptionMessage
			) {

			int value = stream.ReadByte();
			if( value < 0 ) {
				throw new PacketFormatException( packetType, exceptionMessage );
			}

			return (byte)value;
		}

		public static string ReadMqttStringOrThrow(
				this Stream stream,
				PacketType packetType,
				string exceptionMessage
			) {

			int byteLength;
			return ReadMqttStringOrThrow( stream, packetType, exceptionMessage, out byteLength );
		}

		public static string ReadMqttStringOrThrow(
				this Stream stream,
				PacketType packetType,
				string exceptionMessage,
				out int byteLength
			) {

			ushort length = stream.ReadBigEndianOrThrow( packetType, exceptionMessage );

			byte[] bytes = new byte[ length ];
			if( stream.Read( bytes, 0, length ) != length ) {
				throw new PacketFormatException( packetType, exceptionMessage );
			}

			string value = MqttString.GetString( bytes );
			byteLength = ( 2 + length );
			return value;
		}

		public static ushort ReadBigEndianOrThrow(
				this Stream stream,
				PacketType packetType,
				string exceptionMessage
			) {

			byte[] bytes = new byte[ 2 ];
			if( stream.Read( bytes, 0, 2 ) != 2 ) {
				throw new PacketFormatException( packetType, exceptionMessage );
			}

			if( BitConverter.IsLittleEndian ) {
				Array.Reverse( bytes );
			}

			ushort value = BitConverter.ToUInt16( bytes, 0 );
			return value;
		}
	}
}
