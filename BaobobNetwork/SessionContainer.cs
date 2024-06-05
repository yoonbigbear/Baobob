namespace BaobabNetwork
{
	using System.Collections.Concurrent;
	using System.Threading;

	public static class SessionContainer
	{
		public static ConcurrentDictionary<int, TcpSession> Seessions = new ConcurrentDictionary<int, TcpSession>();

		private static int sequenceId = 0;

		public static bool TryAdd(TcpSession tcpSession)
		{
			var id = Interlocked.Increment(ref sequenceId);
			if (Seessions.TryAdd(id, tcpSession))
			{
				tcpSession.SessionId = id;
				return true;
			}
			return false;
		}

		public static bool TryRemoveSession(int sessionId, out TcpSession tcpSession) => Seessions.TryRemove(sessionId, out tcpSession!);
	}
}