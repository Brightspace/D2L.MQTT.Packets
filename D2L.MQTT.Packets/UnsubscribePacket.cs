using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public sealed class UnsubscribePacket : MqttPacket {

		private const PacketType Type = PacketType.Unsubscribe;

		public UnsubscribePacket(
				ushort packetIdentifier,
				IEnumerable<string> topicFilters
			) {

			this.PacketIdentifier = packetIdentifier;
			this.TopicFilters = topicFilters.ToArray();
		}

		public ushort PacketIdentifier { get; }
		public IReadOnlyList<string> TopicFilters { get; }

		public override PacketType PacketType {
			get { return Type; }
		}

		public override void WriteTo( Stream stream ) {

			int length = 2;

			List<byte[]> topics = new List<byte[]>( this.TopicFilters.Count );

			for( int i = 0; i < this.TopicFilters.Count; i++ ) {

				byte[] topic = MqttString.GetBytes( this.TopicFilters[ i ] );
				length += topic.Length + 2;

				topics.Add( topic );
			}

			stream.WriteMqttFixedHeader( Type, 0x02, length );
			stream.WriteBigEndian( this.PacketIdentifier );

			foreach( byte[] topic in topics ) { 
				stream.WriteMqttString( topic );
			}
		}

		internal static UnsubscribePacket Read( MqttFixedHeader header, Stream stream ) {

			ushort packetIdentifier = stream.ReadBigEndianOrThrow( Type, "Missing packet identifier" );
			IEnumerable<string> topics = ReadTopicFilters( header.Length - 2, stream );

			return new UnsubscribePacket(
					packetIdentifier,
					topics
				);
		}

		private static IEnumerable<string> ReadTopicFilters(
				int length,
				Stream stream
			) {

			while( length > 0 ) {

				int topicByteCount;
				string topic = stream.ReadMqttStringOrThrow( Type, "Missing topic filter", out topicByteCount );

				yield return topic;
				length -= ( topicByteCount + 1 );
			}
		}

		public override Task VisitAsync( IAsyncMqttPacketVisitor visitor, CancellationToken cancellationToken ) {
			return visitor.VisitAsync( this, cancellationToken );
		}
	}
}
