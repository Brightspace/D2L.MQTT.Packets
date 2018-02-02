namespace D2L.MQTT.Packets {

	public sealed class MqttFixedHeader {

		public MqttFixedHeader(
				PacketType packetType,
				byte flags,
				int remainingLength
			) {

			this.PacketType = packetType;
			this.Flags = flags;
			this.RemainingLength = remainingLength;
		}

		public PacketType PacketType { get; }
		public byte Flags { get; }
		public int RemainingLength { get; }
	}
}
