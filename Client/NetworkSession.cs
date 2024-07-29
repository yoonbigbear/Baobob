﻿namespace Client
{
	using BaobabNetwork;
	using BaobabNetwork.Tcp;
	using MyGame.Sample;
	using System.Net.Sockets;

	public class NetworkSession : TcpSession
	{
		public NetworkSession(Socket socket, TimeSpan heartbeatInterval, TimeSpan heartbeatTimeout)
			: base(socket, heartbeatInterval, heartbeatTimeout)
		{
			_ = ReadAsync();
		}

		protected override void DeserializeMessage(byte[] buffer, int byteRecevied)
		{
			var payload = new TcpPayload();
			TcpPayload.Deserialize(ref payload, buffer);

			switch (payload.ProtocolId)
			{
				case (int)TcpSession.HeartbeatProtocol.Knock:
					ResponseKnockFromServer();
					return;

				case (int)TcpSession.HeartbeatProtocol.Response:
					CalculateRTT(BitConverter.ToInt64(payload.Data));
					return;
			}

			_ = MessageHandler.Invoke(payload.ProtocolId, Packet.GetRootAsPacket(new Google.FlatBuffers.ByteBuffer(payload.Data)));
		}

		public async Task SendAsync(ReadOnlyMemory<byte> buffer) => await WriteAsync(buffer);
	}
}