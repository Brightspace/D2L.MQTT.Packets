﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public sealed class PubcompPacket : MqttPacket {

		private const PacketType Type = PacketType.Pubcomp;

		public PubcompPacket( ushort packetIdentifier ) {
			this.PacketIdentifier = packetIdentifier;
		}

		public ushort PacketIdentifier { get; }

		public override PacketType PacketType {
			get { return Type; }
		}

		public override void WriteTo( Stream stream ) {

			stream.WriteMqttFixedHeader( Type, 0x00, 2 );
			stream.WriteBigEndian( this.PacketIdentifier );
		}

		internal static PubcompPacket Read( MqttFixedHeader header, Stream stream ) {

			ushort packetIdentifier = stream.ReadBigEndianOrThrow( Type, "Missing packet identifier" );
			return new PubcompPacket( packetIdentifier );
		}

		public override Task VisitAsync( IAsyncMqttPacketVisitor visitor, CancellationToken cancellationToken ) {
			return visitor.VisitAsync( this, cancellationToken );
		}
	}
}
