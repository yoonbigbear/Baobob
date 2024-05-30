namespace Server
{
	using BaobabNetwork;
	using Google.FlatBuffers;
	using MyGame.Sample;
	using System;
	using System.Net.Sockets;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct Payload
	{
		bool Encrypted { get; set; }
		bool Compressed { get; set; }
		int ProtocolId { get; set; }
		int Length { get; set; }
	}

	public class UserSession : TcpSession
	{
		public UserSession(Socket socket) : base(socket)
		{
			_ = ReadAsync();
		}

		protected override void DeserializeMessage(ReadOnlyMemory<byte> buffer, int byteRecevied)
		{
			base.DeserializeMessage(buffer, byteRecevied);

			var packet = Packet.GetRootAsPacket(new ByteBuffer(buffer.ToArray()));
			_ = MessageHandler.Invoke(typeof(Packet).FullName!.GetHashCode(), packet);
		}

		protected override void SerializeMessage(ReadOnlyMemory<byte> buffer)
		{
			base.SerializeMessage(buffer);
		}
	}
}