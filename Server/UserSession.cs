namespace Server
{
	using BaobabNetwork;
	using BaobabNetwork.Tcp;
	using Google.FlatBuffers;
	using MyGame.Sample;
	using System;
	using System.Net.Security;
	using System.Net.Sockets;
	using System.Security.Cryptography.X509Certificates;

	public class UserSession : TcpSession
	{
		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		public UserSession(Socket socket, X509Certificate2 serverSertificate)
			: base(socket, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(4))
		{
			var sslStream = new SslStream(stream, false);
			sslStream.AuthenticateAsServer(
				serverSertificate,
				false,
				System.Security.Authentication.SslProtocols.Tls13,
				false);
			stream = sslStream;

			_ = ReadAsync();
			RepeatKnock();
		}

		protected override void DeserializeMessage(byte[] buffer, int byteRecevied)
		{
			var payload = new TcpPayload();
			TcpPayload.Deserialize(ref payload, buffer);

			switch (payload.ProtocolId)
			{
				case (int)TcpSession.HeartbeatProtocol.Knock:
					_ = CalculateRTT(BitConverter.ToInt64(payload.Data));
					return;

				case (int)TcpSession.HeartbeatProtocol.Response:
					return;

				case (int)TcpSession.HeartbeatProtocol.TimeRequest:
					_ = TimeRequest();
					return;
			}

			_ = MessageHandler.Invoke(payload.ProtocolId, Packet.GetRootAsPacket(new Google.FlatBuffers.ByteBuffer(payload.Data)));

			var builder = new FlatBufferBuilder(128);
			var offset = MyGame.Sample.Packet.CreatePacket(builder, builder.CreateSharedString("Im server"));
			builder.Finish(offset.Value);
			var buf = TcpPayload.Serialize((int)PacketId.Packet, builder.SizedByteArray());
			_ = WriteAsync(buf);
		}
	}
}