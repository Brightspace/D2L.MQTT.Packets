using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public sealed class SubackPacket : MqttPacket {

		private const PacketType Type = PacketType.Suback;

		public SubackPacket(
				ushort packetIdentifier,
				IEnumerable<SubscribeReturnCode> returnCodes
			) {

			this.PacketIdentifier = packetIdentifier;
			this.ReturnCodes = returnCodes.ToArray();
		}

		public ushort PacketIdentifier { get; }
		public IReadOnlyList<SubscribeReturnCode> ReturnCodes;

		public override PacketType PacketType {
			get { return Type; }
		}

		public override void WriteTo( Stream stream ) {

			int length = 2 + this.ReturnCodes.Count;

			stream.WriteMqttFixedHeader( Type, 0x00, length );
			stream.WriteBigEndian( this.PacketIdentifier );

			foreach( SubscribeReturnCode code in this.ReturnCodes ) {
				stream.WriteByte( (byte)code );
			}
		}

		internal static SubackPacket Read( MqttFixedHeader header, Stream stream ) {

			ushort packetIdentifier = stream.ReadBigEndianOrThrow( Type, "Missing packet identifier" );
			IEnumerable<SubscribeReturnCode> codes = ReadReturnCodes( header.Length - 2, stream );

			return new SubackPacket( packetIdentifier, codes );
		}

		private static IEnumerable<SubscribeReturnCode> ReadReturnCodes(
				int length,
				Stream stream
			) {

			for( int i = 0; i < length; i++ ) {

				byte code = stream.ReadByteOrThrow( Type, "Missing return code" );
				yield return (SubscribeReturnCode)code;
			}
		}

		public override Task VisitAsync( IAsyncMqttPacketVisitor visitor, CancellationToken cancellationToken ) {
			return visitor.VisitAsync( this, cancellationToken );
		}
	}
}
