using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public sealed class ConnackPacket : MqttPacket {

		private const PacketType Type = PacketType.Connack;

		public ConnackPacket(
				bool sessionPresent,
				ConnectReturnCode returnCode
			) {

			this.SessionPresent = sessionPresent;
			this.ReturnCode = returnCode;
		}

		public bool SessionPresent { get; }
		public ConnectReturnCode ReturnCode { get; }

		public override PacketType PacketType {
			get { return Type; }
		}

		public override void WriteTo( Stream stream ) {

			stream.WriteMqttFixedHeader( Type, 0x00, 2 );
			stream.WriteByte( GetConnectFlags() );
			stream.WriteByte( (byte)this.ReturnCode );
		}

		internal static ConnackPacket Read( MqttFixedHeader header, Stream stream ) {

			byte connectFlags = stream.ReadByteOrThrow( Type, "Missing connect flags" );
			byte returnCode = stream.ReadByteOrThrow( Type, "Missing return code" );

			return new ConnackPacket(
				sessionPresent: ( connectFlags & 0x01 ) > 0,
				returnCode: (ConnectReturnCode)returnCode
			);
		}

		private byte GetConnectFlags() {

			byte connectFlags = 0x00;

			if( this.SessionPresent ) {
				connectFlags |= 0x01;
			}

			return connectFlags;
		}

		public override Task VisitAsync( IAsyncMqttPacketVisitor visitor, CancellationToken cancellationToken ) {
			return visitor.VisitAsync( this, cancellationToken );
		}
	}
}
