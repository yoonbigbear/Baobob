namespace BaobabNodeNetwork
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Net.Sockets;
	using System.Threading.Tasks;

	public class NodeNetwork : IDisposable
	{
		private Dictionary<EndPoint, TcpClient> nodes = new Dictionary<EndPoint, TcpClient>();

		public void Dispose()
		{
			foreach (var e in nodes.Values)
			{
				e.Dispose();
			}
		}

		public bool TryGet(EndPoint endpoint, out TcpClient tcpClient) => nodes.TryGetValue(endpoint, out tcpClient!);

		public bool TryAdd(EndPoint endpoint, TcpClient tcpClient) => nodes.TryAdd(endpoint, tcpClient);

		public bool TryRemove(EndPoint endpoint, out TcpClient tcpClient) => nodes.Remove(endpoint, out tcpClient!);

		public async Task SendAsync(EndPoint endpoint, byte[] packet)
		{
			if (nodes.TryGetValue(endpoint, out var client))
			{
				try
				{
					await client.GetStream().WriteAsync(packet, 0, packet.Length).ConfigureAwait(false);
				}
				catch (Exception e)
				{
					throw;
				}
			}
		}
	}
}