namespace Server
{
	using BaobabNetwork;
	using MyGame.Sample;
	using System;
	using System.Net.Sockets;

	public class UserSession : TcpSession
	{
		public UserSession(Socket socket) : base(socket)
		{
			_ = ReadAsync();
		}

		protected override void DeserializeMessage(byte[] buffer, int byteRecevied)
		{
			base.DeserializeMessage(buffer, byteRecevied);

			var payload = new Payload();

			Payload.Deserialize(ref payload, buffer);

			var id = typeof(Packet).FullName!.ToString()!.GetHashCode();
			_ = MessageHandler.Invoke(payload.ProtocolId, Packet.GetRootAsPacket(new Google.FlatBuffers.ByteBuffer(payload.Data)));
		}

		protected override void SerializeMessage(ReadOnlyMemory<byte> buffer)
		{
			base.SerializeMessage(buffer);
		}
	}
}