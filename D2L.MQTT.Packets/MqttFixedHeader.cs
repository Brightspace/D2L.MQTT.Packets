using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public sealed class MqttFixedHeader {

		public MqttFixedHeader(
				PacketType packetType,
				byte flags,
				int remainingLength
			) {

			if( packetType == 0 ) {

				throw new ArgumentException(
						paramName: nameof( packetType ),
						message: "Invalid packet type"
					);
			}

			this.PacketType = packetType;
			this.Flags = flags;
			this.RemainingLength = remainingLength;
		}

		public PacketType PacketType { get; }
		public byte Flags { get; }
		public int RemainingLength { get; }

		/// <summary>
		/// Parses the control packet byte of a fixed header.
		/// </summary>
		/// <param name="controlPacketByte">The control packet byte.</param>
		/// <param name="packetType">The packet type.</param>
		/// <param name="flags">The flags specific to the packet type.</param>
		public static void ParseControlPacketByte(
				byte controlPacketByte,
				out PacketType packetType,
				out byte flags
			) {

			packetType = (PacketType)( ( controlPacketByte >> 4 ) & 0x0F );
			if( packetType == 0 ) {
				throw new PacketFormatException( packetType, "Invalid control packet type" );
			}

			flags = (byte)( controlPacketByte & 0x0F );
		}

		/// <summary>
		/// Tries to read the remaining length from the stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <returns>Returns the remaining length if a valid sequence of bytes was provided; otherwise, <c>null</c>.</returns>
		public static int? TryReadRemainingLength( Stream stream ) {

			Func<byte?> nextByteProvider = () => {

				int nextByte = stream.ReadByte();
				if( nextByte < 0 ) {
					return null;
				}

				return (byte)nextByte;
			};

			return TryReadRemainingLength( nextByteProvider );
		}

		/// <summary>
		/// Tries to read the remaining length from the stream.
		/// </summary>
		/// <param name="nextByteProvider">The provider of variable bytes.</param>
		/// <returns>Returns the remaining length if a valid sequence of bytes was provided; otherwise, <c>null</c>.</returns>
		public static int? TryReadRemainingLength(
				Func<byte?> nextByteProvider
			) {

			int length = 0;
			int multiplier = 1;

			for( int i = 0; i < 4; i++ ) {

				byte? nextByte = nextByteProvider();
				if( !nextByte.HasValue ) {
					return null;
				}

				byte encodedByte = nextByte.Value;
				length += ( encodedByte & 127 ) * multiplier;
				multiplier *= 128;

				if( ( encodedByte & 128 ) == 0 ) {
					break;
				}
			}

			return length;
		}

		/// <summary>
		/// Tries to read the remaining length from the stream.
		/// </summary>
		/// <param name="stream">The stream.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>Returns the remaining length if a valid sequence of bytes was provided; otherwise, <c>null</c>.</returns>
		public static Task<int?> TryReadRemainingLengthAsync(
				Stream stream,
				CancellationToken cancellationToken
			) {

			Func<Task<byte?>> nextByteProvider = async () => {

				byte[] buffer = new byte[ 1 ];

				int count = await stream
					.ReadAsync( buffer, 0, 1, cancellationToken )
					.ConfigureAwait( continueOnCapturedContext: false );

				if( count == 0 ) {
					return null;
				}

				return buffer[ 0 ];
			};

			return TryReadRemainingLengthAsync( nextByteProvider );
		}

		/// <summary>
		/// Tries to read the remaining length from the stream.
		/// </summary>
		/// <param name="nextByteProvider">The provider of variable bytes.</param>
		/// <returns>Returns the remaining length if a valid sequence of bytes was provided; otherwise, <c>null</c>.</returns>
		public static async Task<int?> TryReadRemainingLengthAsync(
				Func<Task<byte?>> nextByteProvider
			) {

			int length = 0;
			int multiplier = 1;

			for( int i = 0; i < 4; i++ ) {

				byte? nextByte = await nextByteProvider()
					.ConfigureAwait( continueOnCapturedContext: false );

				if( !nextByte.HasValue ) {
					return null;
				}

				byte encodedByte = nextByte.Value;
				length += ( encodedByte & 127 ) * multiplier;
				multiplier *= 128;

				if( ( encodedByte & 128 ) == 0 ) {
					break;
				}
			}

			return length;
		}

	}
}
