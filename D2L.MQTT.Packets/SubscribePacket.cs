using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public sealed class SubscribePacket : MqttPacket {

		private const PacketType Type = PacketType.Subscribe;

		public SubscribePacket(
				ushort packetIdentifier,
				IEnumerable<Subscription> subscriptions
			) {

			this.PacketIdentifier = packetIdentifier;
			this.Subscriptions = subscriptions.ToArray();
		}

		public ushort PacketIdentifier { get; }
		public IReadOnlyList<Subscription> Subscriptions { get; }

		public override PacketType PacketType {
			get { return Type; }
		}

		public override void WriteTo( Stream stream ) {

			int length = 2;

			List<byte[]> topics = new List<byte[]>( this.Subscriptions.Count );

			for( int i = 0; i < this.Subscriptions.Count; i++ ) {
				Subscription sub = this.Subscriptions[ i ];

				byte[] topic = MqttString.GetBytes( sub.TopicFilter );
				length += topic.Length + 3;

				topics.Add( topic );
			}

			stream.WriteMqttFixedHeader( Type, 0x02, length );
			stream.WriteBigEndian( this.PacketIdentifier );

			for( int i = 0; i < this.Subscriptions.Count; i++ ) {
				Subscription sub = this.Subscriptions[ i ];
				byte[] topic = topics[ i ];

				stream.WriteMqttString( topic );
				stream.WriteByte( (byte)sub.QoS );
			}
		}

		internal static SubscribePacket Read( MqttFixedHeader header, Stream stream ) {

			ushort packetIdentifier = stream.ReadBigEndianOrThrow( Type, "Missing packet identifier" );
			IEnumerable<Subscription> subscriptions = ReadSubscriptions( header.Length - 2, stream );

			return new SubscribePacket(
					packetIdentifier,
					subscriptions
				);
		}

		private static IEnumerable<Subscription> ReadSubscriptions(
				int length,
				Stream stream
			) {

			while( length > 0 ) {

				int topicByteCount;
				string topic = stream.ReadMqttStringOrThrow( Type, "Missing topic filter", out topicByteCount );
				QualityOfService qos = (QualityOfService)stream.ReadByte();

				yield return new Subscription( topic, qos );
				length -= ( topicByteCount + 1 );
			}
		}

		public override Task VisitAsync( IAsyncMqttPacketVisitor visitor, CancellationToken cancellationToken ) {
			return visitor.VisitAsync( this, cancellationToken );
		}
	}
}
