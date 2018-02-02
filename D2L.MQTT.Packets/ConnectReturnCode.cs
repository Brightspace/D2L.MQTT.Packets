namespace D2L.MQTT.Packets {

	public enum ConnectReturnCode : byte {

		ConnectionAccepted = 0x00,
		UnacceptableProtocolVersion = 0x01,
		IdentifierRejected = 0x02,
		ServerUnavailable = 0x03,
		BadUserNameOrPassword = 0x04,
		NotAuthorized = 0x05
	}
}
