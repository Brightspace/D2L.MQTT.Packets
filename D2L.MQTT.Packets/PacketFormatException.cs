using System;

namespace D2L.MQTT.Packets {

	public sealed class PacketFormatException : Exception {

		public PacketFormatException(
				PacketType packetType,
				string message
			)
			: base( message ) {

			this.PacketType = packetType;
		}

		public PacketType PacketType { get; }
	}
}
