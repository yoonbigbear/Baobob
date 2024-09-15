namespace Server
{
	using BaobabNetwork;
	using BaobobCore;
	using System.Net;
	using System.Net.Sockets;
	using System.Security.Cryptography.X509Certificates;

	public class GameServer : ServerBuilder
	{
		private X509Certificate2 serverCertificate;

		public GameServer(IPAddress ip, short port) : base(ip, port)
		{
			serverCertificate = new X509Certificate2("server.pfx", "1234567890");
		}

		public override void AcceptSession(Socket? socket)
		{
			base.AcceptSession(socket);

			var userSession = new UserSession(socket!, serverCertificate);

			if (!SessionContainer.TryAdd(userSession))
			{
				throw new BaobabNetworkException();
			}
			Logger.Debug($"Client Connected {userSession.SessionId}");
		}
	}
}