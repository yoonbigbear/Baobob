namespace Client
{
	using BaobabNetwork;
	using BaobabNetwork.Tcp;
	using MyGame.Sample;
	using System.Net.Security;
	using System.Net.Sockets;
	using System.Security.Authentication;
	using System.Security.Cryptography.X509Certificates;

	public class NetworkSession : TcpSession
	{
		public NetworkSession(Socket socket, TimeSpan heartbeatInterval, TimeSpan heartbeatTimeout)
			: base(socket, heartbeatInterval, heartbeatTimeout)
		{
			var sslstream = new SslStream(stream, false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
			sslstream.AuthenticateAsClient("localhost", null, SslProtocols.Tls13, false);
			stream = sslstream;

			_ = ReadAsync();
		}

		public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true; // 실제 환경에서는 올바른 검증 로직을 구현해야 함
		}

		protected override void DeserializeMessage(byte[] buffer, int byteRecevied)
		{
			var payload = new TcpPayload();
			TcpPayload.Deserialize(ref payload, buffer);

			switch (payload.ProtocolId)
			{
				case (int)TcpSession.HeartbeatProtocol.Knock:
					_ = ResponseKnockFromServer();
					return;

				case (int)TcpSession.HeartbeatProtocol.Response:
					_ = CalculateRTT(BitConverter.ToInt64(payload.Data));
					_ = TimeRequest();
					return;

				case (int)TcpSession.HeartbeatProtocol.TimeRequest:
					EstimateServerTime(BitConverter.ToInt64(payload.Data));
					return;
			}

			_ = MessageHandler.Invoke(payload.ProtocolId, Packet.GetRootAsPacket(new Google.FlatBuffers.ByteBuffer(payload.Data)));
		}

		public async Task SendAsync(ReadOnlyMemory<byte> buffer) => await WriteAsync(buffer).ConfigureAwait(false);
	}
}