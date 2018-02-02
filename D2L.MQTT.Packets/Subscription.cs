namespace D2L.MQTT.Packets {

	public sealed class Subscription {

		public Subscription(
				string topicFilter,
				QualityOfService qos
			) {

			this.TopicFilter = topicFilter;
			this.QoS = qos;
		}

		public string TopicFilter { get; }
		public QualityOfService QoS { get; }
	}
}
