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
			var userSession = new UserSession(socket!);

			if (!SessionContainer.TryAdd(userSession))
			{
				throw new BaobabNetworkException();
			}
			Logger.Debug($"Client Connected {userSession.SessionId}");
		}
	}
}