using System;

namespace D2L.MQTT.Packets {

	public sealed class ConnectionWill {

		public ConnectionWill(
				string topic,
				byte[] message,
				QualityOfService qos,
				bool retain
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
			this.Retain = retain;
		}

		public string Topic { get; }
		public byte[] Message { get; }
		public QualityOfService QoS { get; }
		public bool Retain { get; }
	}
}
