using System;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	internal sealed class ConnectionWillTests {

		[Test]
		public void ConnectionWill_InvalidTopic_ShouldThrow(
				[Values( null, "")]
				string topic
			) {

			Assert.Throws<ArgumentNullException>( () => {
				new ConnectionWill(
					topic: topic,
					message: new byte[ 0 ],
					qos: QualityOfService.QoS0,
					retain: false
				);
			} );
		}

		[Test]
		public void ConnectionWill_NullMessage_ShouldThrow() {

			Assert.Throws<ArgumentNullException>( () => {
				new ConnectionWill(
					topic: "abc",
					message: null,
					qos: QualityOfService.QoS0,
					retain: false
				);
			} );
		}
	}
}
