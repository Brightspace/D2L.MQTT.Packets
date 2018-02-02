namespace D2L.MQTT.Packets {

	public enum QualityOfService : byte {

		/// <summary>
		/// At Most Once
		/// </summary>
		QoS0 = 0x00,

		/// <summary>
		/// At Least Once
		/// </summary>
		QoS1 = 0x01,

		/// <summary>
		/// Exactly Once
		/// </summary>
		QoS2 = 0x02
	}
}
