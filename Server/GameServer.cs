namespace Server
{
	using BaobabNetwork;
	using BaobobCore;
	using System.Net;
	using System.Net.Sockets;

	public class GameServer : ServerBuilder
	{
		public GameServer(IPAddress ip, short port) : base(ip, port)
		{
		}

		public override void AcceptSession(Socket? socket)
		{
			base.AcceptSession(socket);
			if (!SessionFactory.TryCreateSession(socket!, out TcpSession tcpSession))
			{
				throw new BaobabNetworkException();
			}

			var userSession = tcpSession as UserSession;

			Logger.Debug($"Client Connected {tcpSession.SessionId}");
		}
	}
}