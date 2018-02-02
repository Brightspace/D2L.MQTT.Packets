using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public abstract class MqttPacket {

		public abstract PacketType PacketType { get; }

		public abstract void WriteTo( Stream stream );

		public abstract Task VisitAsync( IAsyncMqttPacketVisitor visitor, CancellationToken cancellationToken );
	}
}
