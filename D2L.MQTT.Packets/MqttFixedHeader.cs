namespace D2L.MQTT.Packets {

	internal sealed class MqttFixedHeader {

		public MqttFixedHeader(
				PacketType packetType,
				byte flags,
				int length
			) {

			this.PacketType = packetType;
			this.Flags = flags;
			this.Length = length;
		}

		public PacketType PacketType { get; }
		public byte Flags { get; }
		public int Length { get; }
	}
}
