using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	public sealed class MqttPacketTestCase<TPacket>
		: TestCaseData
		where TPacket : MqttPacket {

		public MqttPacketTestCase( string name, TPacket packet, byte[] bytes )
			: base( packet, bytes ) {

			this.SetName( name );
		}
	}
}
