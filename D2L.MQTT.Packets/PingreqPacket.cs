using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public sealed class PingreqPacket : MqttPacket {

		private const PacketType Type = PacketType.Pingreq;

		public ushort PacketIdentifier { get; }

		public override PacketType PacketType {
			get { return Type; }
		}

		public override void WriteTo( Stream stream ) {
			stream.WriteMqttFixedHeader( Type, 0x00, 0 );
		}

		internal static PingreqPacket Read( MqttFixedHeader header, Stream stream ) {
			return new PingreqPacket();
		}

		public override Task VisitAsync( IAsyncMqttPacketVisitor visitor, CancellationToken cancellationToken ) {
			return visitor.VisitAsync( this, cancellationToken );
		}
	}
}
