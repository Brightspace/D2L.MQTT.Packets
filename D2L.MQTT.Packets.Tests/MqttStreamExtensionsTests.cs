using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class MqttStreamExtensionsTests {

		private static IEnumerable<TestCaseData> GetVariableLengthTestCases() {

			yield return new TestCaseData( 0, new byte[] { 0x00 } );
			yield return new TestCaseData( 127, new byte[] { 0x7F } );
			yield return new TestCaseData( 128, new byte[] { 0x80, 0x01 } );
			yield return new TestCaseData( 16383, new byte[] { 0xFF, 0x7F } );
			yield return new TestCaseData( 16384, new byte[] { 0x80, 0x80, 0x01 } );
			yield return new TestCaseData( 2097151, new byte[] { 0xFF, 0xFF, 0x7F } );
			yield return new TestCaseData( 2097152, new byte[] { 0x80, 0x80, 0x80, 0x01 } );
			yield return new TestCaseData( 268435455, new byte[] { 0xFF, 0xFF, 0xFF, 0x7F } );
		}

		[TestCaseSource( nameof( GetVariableLengthTestCases ) )]
		public void ReadVariableLength(
				int expectedLength,
				byte[] bytes
			) {

			using( MemoryStream ms = new MemoryStream( bytes ) ) {

				int? actualLength = ms.ReadVariableLength();
				Assert.AreEqual( expectedLength, actualLength );
			}
		}

		[TestCaseSource( nameof( GetVariableLengthTestCases ) )]
		public void WriteVariableLength(
				int length,
				byte[] expectedBytes
			) {

			using( MemoryStream ms = new MemoryStream() ) {
				ms.WriteVariableLength( length );

				byte[] actualBytes = ms.ToArray();
				CollectionAssert.AreEqual( expectedBytes, actualBytes );
			}
		}
	}
}
