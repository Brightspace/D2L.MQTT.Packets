using System;
using System.Text;

namespace D2L.MQTT.Packets {

	internal static class MqttString {

		private static readonly Encoding Encoding = Encoding.UTF8;

		internal static byte[] GetBytes( string value ) {

			if( value == null ) {
				throw new ArgumentNullException( nameof( value ) );
			}

			if( value.Length > ushort.MaxValue ) {
				throw new ArgumentOutOfRangeException( nameof( value ), "String too long for MQTT" );
			}

			return Encoding.GetBytes( value );
		}

		internal static string GetString( byte[] bytes ) {
			return Encoding.GetString( bytes );
		}
	}
}
