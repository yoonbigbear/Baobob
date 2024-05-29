namespace BaobabNetwork
{
	using System.Collections.Concurrent;
	using System.Net.Sockets;

	public static class SessionFactory
	{
		public static ConcurrentDictionary<int, TcpSession> Seessions = new();

		private static int sequenceId = 0;

		public static bool TryCreateSession(Socket clientSocket, out TcpSession tcpSession)
		{
			tcpSession = new TcpSession(Interlocked.Increment(ref sequenceId), clientSocket);
			return Seessions.TryAdd(tcpSession.SessionId, tcpSession);
		}

		public static bool TryRemoveSession(int sessionId, out TcpSession tcpSession)
		{
			return Seessions.TryRemove(sessionId, out tcpSession!);
		}
	}
}