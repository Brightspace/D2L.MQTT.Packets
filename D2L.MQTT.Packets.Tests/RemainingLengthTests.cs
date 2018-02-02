using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace D2L.MQTT.Packets.Tests {

	[TestFixture]
	public sealed class RemainingLengthTests {

		private static IEnumerable<TestCaseData> GetRemainingLengthTestCases() {

			yield return new TestCaseData( 0, new byte[] { 0x00 } );
			yield return new TestCaseData( 127, new byte[] { 0x7F } );
			yield return new TestCaseData( 128, new byte[] { 0x80, 0x01 } );
			yield return new TestCaseData( 16383, new byte[] { 0xFF, 0x7F } );
			yield return new TestCaseData( 16384, new byte[] { 0x80, 0x80, 0x01 } );
			yield return new TestCaseData( 2097151, new byte[] { 0xFF, 0xFF, 0x7F } );
			yield return new TestCaseData( 2097152, new byte[] { 0x80, 0x80, 0x80, 0x01 } );
			yield return new TestCaseData( 268435455, new byte[] { 0xFF, 0xFF, 0xFF, 0x7F } );
		}

		[TestCaseSource( nameof( GetRemainingLengthTestCases ) )]
		public void TryReadRemainingLength(
				int expectedLength,
				byte[] bytes
			) {

			using( MemoryStream ms = new MemoryStream( bytes ) ) {

				int? remainingLength = MqttFixedHeader.TryReadRemainingLength( ms );
				Assert.IsTrue( remainingLength.HasValue );
				Assert.AreEqual( expectedLength, remainingLength.Value );
			}
		}

		[TestCaseSource( nameof( GetRemainingLengthTestCases ) )]
		public async Task TryReadRemainingLengthAsync(
				int expectedLength,
				byte[] bytes
			) {

			using( MemoryStream ms = new MemoryStream( bytes ) ) {

				int? remainingLength = await MqttFixedHeader.TryReadRemainingLengthAsync( ms, CancellationToken.None );
				Assert.IsTrue( remainingLength.HasValue );
				Assert.AreEqual( expectedLength, remainingLength.Value );
			}
		}

		[TestCaseSource( nameof( GetRemainingLengthTestCases ) )]
		public void WriteRemainingLength(
				int length,
				byte[] expectedBytes
			) {

			using( MemoryStream ms = new MemoryStream() ) {
				ms.WriteRemainingLength( length );

				byte[] actualBytes = ms.ToArray();
				CollectionAssert.AreEqual( expectedBytes, actualBytes );
			}
		}

		private static IEnumerable<TestCaseData> GetIncompleteRemainingLengthTestCases() {

			yield return new TestCaseData( new byte[ 0 ] );
			yield return new TestCaseData( new byte[] { 0xFF } );
			yield return new TestCaseData( new byte[] { 0xFF, 0xFF } );
			yield return new TestCaseData( new byte[] { 0xFF, 0xFF, 0xFF } );
		}

		[TestCaseSource( nameof( GetIncompleteRemainingLengthTestCases ) )]
		public void TryReadRemainingLength_WhenIncomplete(
				byte[] bytes 
			) {

			using( MemoryStream ms = new MemoryStream( bytes ) ) {

				int? remainingLength = MqttFixedHeader.TryReadRemainingLength( ms );
				Assert.IsFalse( remainingLength.HasValue );
			}
		}

		[TestCaseSource( nameof( GetIncompleteRemainingLengthTestCases ) )]
		public async Task TryReadRemainingLengthAsync_WhenIncomplete(
				byte[] bytes
			) {

			using( MemoryStream ms = new MemoryStream( bytes ) ) {

				int? remainingLength = await MqttFixedHeader.TryReadRemainingLengthAsync( ms, CancellationToken.None );
				Assert.IsFalse( remainingLength.HasValue );
			}
		}
	}
}
