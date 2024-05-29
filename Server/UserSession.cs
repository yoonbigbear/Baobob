namespace Server
{
	using BaobabNetwork;
	using System;
	using System.Net.Sockets;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct Payload
	{
	}

	public class UserSession : TcpSession
	{
		public UserSession(int sessionId, Socket socket) : base(sessionId, socket)
		{
			_ = ReadAsync();
		}

		protected override void DeserializeMessage(ReadOnlyMemory<byte> buffer, int byteRecevied)
		{
			base.DeserializeMessage(buffer, byteRecevied);
		}

		protected override void SerializeMessage(ReadOnlyMemory<byte> buffer)
		{
			base.SerializeMessage(buffer);
		}
	}
}