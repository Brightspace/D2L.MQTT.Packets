using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public sealed class PublishPacket : MqttPacket {

		private const PacketType Type = PacketType.Publish;

		public PublishPacket(
				string topic,
				byte[] message,
				QualityOfService qos,
				ushort packetIdentifier,
				bool retain,
				bool duplicate
			) {

			if( string.IsNullOrEmpty( topic ) ) {
				throw new ArgumentNullException( nameof( topic ) );
			}

			if( message == null ) {
				throw new ArgumentNullException( nameof( message ) );
			}

			this.Topic = topic;
			this.Message = message;
			this.QoS = qos;
			this.PacketIdentifier = IncludePacketIdentifier( qos ) ? (ushort?)packetIdentifier : null;
			this.Retain = retain;
			this.Duplicate = duplicate;
		}

		public string Topic { get; }
		public byte[] Message { get; }
		public QualityOfService QoS { get; }
		public ushort? PacketIdentifier { get; }
		public bool Retain { get; }
		public bool Duplicate { get; }

		public override PacketType PacketType {
			get { return Type; }
		}

		public override void WriteTo( Stream stream ) {

			PublishFlags flags = GetPublishFlags();

			int length = 0;

			byte[] topic = MqttString.GetBytes( this.Topic );
			length += ( topic.Length + 2 );

			if( this.PacketIdentifier.HasValue ) {
				length += 2;
			}

			length += ( this.Message.Length );

			// ---------------------------------------------------------

			stream.WriteMqttFixedHeader( Type, (byte)flags, length );

			stream.WriteMqttString( topic );

			if( this.PacketIdentifier.HasValue ) {
				stream.WriteBigEndian( this.PacketIdentifier.Value );
			}

			stream.Write( this.Message, 0, this.Message.Length );
		}

		internal static PublishPacket Read( MqttFixedHeader header, Stream stream ) {

			PublishFlags publishFlags = (PublishFlags)header.Flags;

			bool retain = publishFlags.HasFlag( PublishFlags.Retain );
			QualityOfService qos = (QualityOfService)( ( (byte)publishFlags >> 1 ) & 0x03 );
			bool duplicate = publishFlags.HasFlag( PublishFlags.Duplicate );

			int messageLength = header.Length;

			int topicBytes;
			string topic = stream.ReadMqttStringOrThrow( Type, "Missing topic", out topicBytes );
			messageLength -= topicBytes;

			ushort packetIdentifier = 0;
			if( IncludePacketIdentifier( qos ) ) {
				packetIdentifier = stream.ReadBigEndianOrThrow( Type, "Missing packet identifier" );
				messageLength -= 2;
			}

			byte[] messageBuffer = new byte[ messageLength ];
			if( stream.Read( messageBuffer, 0, messageLength ) != messageLength ) {
				throw new PacketFormatException( Type, "Incomplete message" );
			}

			return new PublishPacket(
					topic: topic,
					message: messageBuffer,
					qos: qos,
					packetIdentifier: packetIdentifier,
					retain: retain,
					duplicate: duplicate
				);
		}

		[Flags]
		private enum PublishFlags : byte {
			None = 0x00,
			Retain = 0x01,
			QoS0 = ( QualityOfService.QoS0 << 1 ),
			QoS1 = ( QualityOfService.QoS1 << 1 ),
			QoS2 = ( QualityOfService.QoS2 << 1 ),
			Duplicate = 0x01 << 3
		}

		private PublishFlags GetPublishFlags() {

			PublishFlags flags = PublishFlags.None;

			if( this.Retain ) {
				flags |= PublishFlags.Retain;
			}

			flags |= (PublishFlags)( (byte)this.QoS << 1 );

			if( this.Duplicate ) {
				flags |= PublishFlags.Duplicate;
			}

			return flags;
		}

		private static bool IncludePacketIdentifier( QualityOfService qos ) {
			return ( qos == QualityOfService.QoS1 || qos == QualityOfService.QoS2 );
		}

		public override Task VisitAsync( IAsyncMqttPacketVisitor visitor, CancellationToken cancellationToken ) {
			return visitor.VisitAsync( this, cancellationToken );
		}
	}
}
