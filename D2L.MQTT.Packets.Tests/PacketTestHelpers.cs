using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	public static class PacketTestHelpers {

		public static TPacket Read<TPacket>( byte[] bytes )
			where TPacket : MqttPacket {

			using( MemoryStream ms = new MemoryStream( bytes ) ) {

				MqttPacket packet = MqttPacketReader.ReadPacket( ms );
				Assert.IsInstanceOf<TPacket>( packet );
				return (TPacket)packet;
			}
		}

		public static void AssertBytes( MqttPacket packet, byte[] expectedBytes ) {

			byte[] actualBytes = GetBytes( packet );
			CollectionAssert.AreEqual( expectedBytes, actualBytes );
		}

		private static byte[] GetBytes( MqttPacket packet ) {
			using( MemoryStream ms = new MemoryStream() ) {
				packet.WriteTo( ms );
				return ms.ToArray();
			}
		}

		public static IEnumerable<TestCaseData> Prefix(
				this IEnumerable<TestCaseData> testCases,
				string prefix
			) {

			foreach( TestCaseData tc in testCases ) {
				tc.SetName( prefix + tc.TestName );
				yield return tc;
			}
		}
	}
}
