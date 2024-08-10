namespace BaobabNodeNetwork
{
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;

	public class NodeNetwork : IDisposable
	{
		private ConcurrentDictionary<EndPoint, TcpClient> channels = new ConcurrentDictionary<EndPoint, TcpClient>();

		public void Dispose()
		{
			foreach (var e in channels.Values)
			{
				e.Dispose();
			}
		}

		public bool TryAdd(EndPoint endpoint, TcpClient tcpClient) => channels.TryAdd(endpoint, tcpClient);

		public bool TryRemove(EndPoint endpoint, out TcpClient tcpClient) => channels.TryRemove(endpoint, out tcpClient!);
	}
}