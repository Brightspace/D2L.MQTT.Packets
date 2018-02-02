using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace D2L.MQTT.Packets {

	public sealed class ConnectPacket : MqttPacket {

		private const PacketType Type = PacketType.Connect;

		public ConnectPacket(
				MqttProtocolLevel protocolLevel,
				string protocolName,
				string clientId,
				bool cleanSession,
				ushort keepAlive,
				string userName,
				string password,
				ConnectionWill will
			) {

			if( String.IsNullOrEmpty( protocolName ) ) {
				throw new ArgumentNullException( nameof( protocolName ) );
			}

			if( clientId == null ) {
				throw new ArgumentNullException( nameof( clientId ) );
			}

			this.ProtocolLevel = protocolLevel;
			this.ProtocolName = protocolName;
			this.ClientId = clientId;
			this.CleanSession = cleanSession;
			this.KeepAlive = keepAlive;
			this.UserName = userName;
			this.Password = password;
			this.Will = will;
		}

		public MqttProtocolLevel ProtocolLevel { get; }
		public string ProtocolName { get; }
		public string ClientId { get; }
		public bool CleanSession { get; }
		public ushort KeepAlive { get; }
		public string UserName { get; }
		public string Password { get; }
		public ConnectionWill Will { get; }

		public override PacketType PacketType {
			get { return Type; }
		}

		public override void WriteTo( Stream stream ) {

			ConnectFlags connectFlags = GetConnectFlags();

			int length = (

				// Protocol Level
				+1

				// Connect flags
				+ 1

				// Keep Alive
				+ 2
			);

			byte[] protocolName = MqttString.GetBytes( this.ProtocolName );
			length += protocolName.Length + 2;

			byte[] clientId = MqttString.GetBytes( this.ClientId ?? string.Empty );
			length += clientId.Length + 2;

			byte[] willTopic = null;
			byte[] willMessage = null;

			if( this.Will != null ) {

				willTopic = MqttString.GetBytes( this.Will.Topic );
				length += willTopic.Length + 2;

				willMessage = this.Will.Message;
				length += willMessage.Length + 2;
			}

			byte[] userName = null;
			if( this.UserName != null ) {
				userName = MqttString.GetBytes( this.UserName );
				length += userName.Length + 2;
			}

			byte[] password = null;
			if( this.Password != null ) {
				password = MqttString.GetBytes( this.Password );
				length += password.Length + 2;
			}

			// ---------------------------------------------------------

			stream.WriteMqttFixedHeader( Type, 0x0, length );

			stream.WriteMqttString( protocolName );
			stream.WriteByte( (byte)this.ProtocolLevel );
			stream.WriteByte( (byte)connectFlags );
			stream.WriteBigEndian( this.KeepAlive );

			stream.WriteMqttStringIfNotNull( clientId );
			stream.WriteMqttStringIfNotNull( willTopic );
			stream.WriteMqttStringIfNotNull( willMessage );
			stream.WriteMqttStringIfNotNull( userName );
			stream.WriteMqttStringIfNotNull( password );
		}

		internal static ConnectPacket Read( MqttFixedHeader header, Stream stream ) {

			string protocolName = stream.ReadMqttStringOrThrow( Type, "Missing protocol name" );
			MqttProtocolLevel protocolLevel = (MqttProtocolLevel)stream.ReadByteOrThrow( Type, "Missing protocol level" );
			ConnectFlags connectFlags = (ConnectFlags)stream.ReadByteOrThrow( Type, "Missing connect flags" );
			ushort keepAlive = stream.ReadBigEndianOrThrow( Type, "Missing return code" );

			string clientId = stream.ReadMqttStringOrThrow( Type, "Missing client id" );

			ConnectionWill will = null;
			if( connectFlags.HasFlag( ConnectFlags.WillFlag ) ) {

				string willTopic = stream.ReadMqttStringOrThrow( Type, "Missing will topic" );

				ushort willMessageLength = stream.ReadBigEndianOrThrow( Type, "Missing will message length" );
				byte[] willMessage = new byte[ willMessageLength ];
				if( stream.Read( willMessage, 0, willMessageLength ) != willMessageLength ) {
					throw new PacketFormatException( Type, "Missing will message" );
				}

				QualityOfService willQoS = (QualityOfService)( ( (byte)connectFlags >> 3 ) & 0x03 );
				bool willRetain = connectFlags.HasFlag( ConnectFlags.WillRetain );

				will = new Packets.ConnectionWill(
						topic: willTopic,
						message: willMessage,
						qos: willQoS,
						retain: willRetain
					);
			}

			string userName = null;
			if( connectFlags.HasFlag( ConnectFlags.UserName ) ) {
				userName = stream.ReadMqttStringOrThrow( Type, "Missing user name" );
			}

			string password = null;
			if( connectFlags.HasFlag( ConnectFlags.Password ) ) {
				password = stream.ReadMqttStringOrThrow( Type, "Missing password" );
			}

			return new ConnectPacket(
					protocolLevel: protocolLevel,
					protocolName: protocolName,
					clientId: clientId,
					cleanSession: connectFlags.HasFlag( ConnectFlags.CleanSession ),
					keepAlive: keepAlive,
					userName: userName,
					password: password,
					will: will
				);
		}

		[Flags]
		private enum ConnectFlags : byte {
			None = 0x00,
			CleanSession = ( 0x01 << 1 ),
			WillFlag = ( 0x01 << 2 ),
			WillQoS0 = ( QualityOfService.QoS0 << 3 ),
			WillQoS1 = ( QualityOfService.QoS1 << 3 ),
			WillQoS2 = ( QualityOfService.QoS2 << 3 ),
			WillRetain = ( 0x01 << 5 ),
			Password = ( 0x01 << 6 ),
			UserName = ( 0x01 << 7 )
		}

		private ConnectFlags GetConnectFlags() {

			ConnectFlags flags = ConnectFlags.None;

			if( this.CleanSession ) {
				flags |= ConnectFlags.CleanSession;
			}

			if( this.Will != null ) {

				flags |= ConnectFlags.WillFlag;
				flags |= (ConnectFlags)( (byte)this.Will.QoS << 3 );

				if( this.Will.Retain ) {
					flags |= ConnectFlags.WillRetain;
				}
			}

			if( this.Password != null ) {
				flags |= ConnectFlags.Password;
			}

			if( this.UserName != null ) {
				flags |= ConnectFlags.UserName;
			}

			return flags;
		}

		public override Task VisitAsync( IAsyncMqttPacketVisitor visitor, CancellationToken cancellationToken ) {
			return visitor.VisitAsync( this, cancellationToken );
		}
	}
}
