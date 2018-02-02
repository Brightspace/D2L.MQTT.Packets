namespace D2L.MQTT.Packets {

	public enum PacketType : byte {

		Connect = 1,
		Connack = 2,

		Publish = 3,
		Puback = 4,
		Pubrec = 5,
		Pubrel = 6,
		Pubcomp = 7,

		Subscribe = 8,
		Suback = 9,

		Unsubscribe = 10,
		Unsuback = 11,

		Pingreq = 12,
		Pingresp = 13,

		Disconnect = 14
	}
}
