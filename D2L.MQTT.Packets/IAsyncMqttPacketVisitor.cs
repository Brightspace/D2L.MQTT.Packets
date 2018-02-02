using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public interface IAsyncMqttPacketVisitor {

		Task VisitAsync( ConnectPacket packet, CancellationToken cancellationToken );
		Task VisitAsync( ConnackPacket packet, CancellationToken cancellationToken );

		Task VisitAsync( PublishPacket packet, CancellationToken cancellationToken );
		Task VisitAsync( PubackPacket packet, CancellationToken cancellationToken );

		Task VisitAsync( PubrecPacket packet, CancellationToken cancellationToken );
		Task VisitAsync( PubrelPacket packet, CancellationToken cancellationToken );
		Task VisitAsync( PubcompPacket packet, CancellationToken cancellationToken );

		Task VisitAsync( SubscribePacket packet, CancellationToken cancellationToken );
		Task VisitAsync( SubackPacket packet, CancellationToken cancellationToken );

		Task VisitAsync( UnsubscribePacket packet, CancellationToken cancellationToken );
		Task VisitAsync( UnsubackPacket packet, CancellationToken cancellationToken );

		Task VisitAsync( PingreqPacket packet, CancellationToken cancellationToken );
		Task VisitAsync( PingrespPacket packet, CancellationToken cancellationToken );

		Task VisitAsync( DisconnectPacket packet, CancellationToken cancellationToken );

	}
}
