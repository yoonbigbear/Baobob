using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BaobabNodeNetwork
{
	public class NodeChannel : IDisposable
	{
		public TcpListener listener;
		private const int PortBase = 5000;
		private TcpClient client;
		private NetworkStream stream;
		private NodeNetwork nodeNetwork = new NodeNetwork();

		public void Start(int id)
		{
			listener = new TcpListener(System.Net.IPAddress.Any, PortBase + id);
			listener.Start();
		}

		public async Task Accept()
		{
			TcpClient client = await listener.AcceptTcpClientAsync();
			NetworkStream stream = client.GetStream();
			nodeNetwork.TryAdd(client.Client.RemoteEndPoint, client);
		}

		public async Task SendMessage(byte[] buffer)
		{
			await stream?.WriteAsync(buffer, 0, buffer.Length);
		}

		public async Task<int> ReadMessage(byte[] buffer)
		{
			var task = stream.ReadAsync(buffer, 0, buffer.Length);
			if (await Task.WhenAny(task, Task.Delay(2000)) == task)
			{
				return task.Result;
			}
			return await Task.FromResult(0);
		}

		public void Dispose()
		{
			client?.Dispose();
			stream?.Dispose();
			listener?.Stop();
		}
	}
}