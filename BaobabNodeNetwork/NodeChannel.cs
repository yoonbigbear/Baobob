using BaobobCore;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace BaobabNodeNetwork
{
	public class NodeChannel : IDisposable
	{
		private const int PortBase = 5000;
		private NodeNetwork nodeNetwork = new NodeNetwork();
		private Dictionary<int, IPEndPoint> nodeKeys = new Dictionary<int, IPEndPoint>();

		private readonly TcpListener listener;

		public event Action<TcpClient> ClientConnectedSuccessfully;

		public event Action<IPEndPoint> HeartbeatTimeout;

		private ArrayPool<byte> pool = ArrayPool<byte>.Shared;

		private int reconnectDelay = 1000;
		private int nodeId;
		private CancellationTokenSource ctsClose = new CancellationTokenSource();

		private int hearbeatInterval;
		private int heartbeatTimeout;

		private DateTime lastHeartbeatChecked;

		public NodeChannel(int id, IEnumerable<(int, IPEndPoint)> nodes, int reconnectMs = 1000)
		{
			nodeId = id;
			reconnectDelay = reconnectMs;
			listener = new TcpListener(System.Net.IPAddress.Any, PortBase + id);

			// Add nodes
			foreach (var item in nodes)
			{
				nodeKeys.TryAdd(item.Item1, item.Item2);
			}

			ClientConnectedSuccessfully += (client) =>
			{
				Console.WriteLine($"{nodeId} Client accepted {client.Client.RemoteEndPoint}");
			};

			HeartbeatTimeout += (endpoint) =>
			{
				Console.WriteLine($"{nodeId} Heartbeat timeout {endpoint}");
			};
		}

		public void Start(int hearbeatInterval, int heartbeatTimeout)
		{
			Task.Factory.StartNew(async () => await AcceptClientsAsync().ConfigureAwait(false));

			this.hearbeatInterval = hearbeatInterval;
			this.heartbeatTimeout = heartbeatTimeout;

			// Connect to nodes with lower id
			foreach (var e in nodeKeys)
			{
				if (e.Key < nodeId)
				{
					_ = TryConnect(e.Value);
				}
			}
		}

		private async Task AcceptClientsAsync()
		{
			// Accept clients
			listener.Start();
			while (!ctsClose.IsCancellationRequested)
			{
				var client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
				nodeNetwork.TryAdd(client.Client.RemoteEndPoint!, client);
				ClientConnectedSuccessfully?.Invoke(client);

				_ = Task.Factory.StartNew(async () =>
				{
					lastHeartbeatChecked = DateTime.Now;
					while (true)
					{
						var now = DateTime.Now;
						if (now - lastHeartbeatChecked > TimeSpan.FromMilliseconds(heartbeatTimeout))
						{
							Console.WriteLine($"{nodeId} Heartbeat timeout");
							break;
						}

						await Task.Delay(hearbeatInterval).ConfigureAwait(false);
					}
				}, ctsClose.Token);
				_ = Task.Factory.StartNew(() => { _ = ReadAsync(client.Client.RemoteEndPoint as IPEndPoint); }, ctsClose.Token).ConfigureAwait(false);
			}
		}

		public async Task TryConnect(IPEndPoint endPoint)
		{
			TcpClient client = new TcpClient();
			await client.ConnectAsync(endPoint.Address, endPoint.Port).ConfigureAwait(false);
			while (!client.Connected)
			{
				await Task.Delay(reconnectDelay).ConfigureAwait(false);
				Console.WriteLine($"{nodeId} Reconnecting to {endPoint}");
				await client.ConnectAsync(endPoint.Address, endPoint.Port).ConfigureAwait(false);
			}

			ClientConnectedSuccessfully?.Invoke(client);
			nodeNetwork.TryAdd(endPoint, client);

			// Start heartbeat
			_ = Task.Factory.StartNew(async () =>
			{
				lastHeartbeatChecked = DateTime.Now;
				while (true)
				{
					var now = DateTime.Now;
					if (now - lastHeartbeatChecked > TimeSpan.FromMilliseconds(heartbeatTimeout))
					{
						Console.WriteLine($"{nodeId} Heartbeat timeout");
						break;
					}

					await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes("heartbeatRequest")).ConfigureAwait(false);
					await Task.Delay(hearbeatInterval).ConfigureAwait(false);
				}
			}, ctsClose.Token);

			// Start read
			_ = Task.Factory.StartNew(() => { _ = ReadAsync(endPoint); }, ctsClose.Token).ConfigureAwait(false);
		}

		public async Task SendAsync(int id, byte[] buffer)
		{
			if (!nodeKeys.TryGetValue(id, out var endpoint))
			{
				return;
			}

			await nodeNetwork.SendAsync(endpoint, buffer).ConfigureAwait(false);
		}

		public async Task ReadAsync(IPEndPoint endPoint)
		{
			if (!nodeNetwork.TryGet(endPoint, out var client))
			{
				return;
			}

			while (!ctsClose.IsCancellationRequested)
			{
				var buffer = ArrayPool<byte>.Shared.Rent(1024);
				var bytes = await client.GetStream().ReadAsync(buffer).ConfigureAwait(false);
				if (bytes == 0)
				{
					break;
				}
				await ReadPacketAsync(buffer, bytes, client).ConfigureAwait(false);
				ArrayPool<byte>.Shared.Return(buffer);
			}
		}

		protected virtual async Task ReadPacketAsync(byte[] buffer, int byteRecevied, TcpClient client)
		{
			var packet = Encoding.UTF8.GetString(buffer, 0, byteRecevied);
			Console.WriteLine($"Received packet: {packet}");
			switch (packet)
			{
				case "heartbeatRequest":
					{
						lastHeartbeatChecked = DateTime.Now;
						await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes("heartbeatResponse")).ConfigureAwait(false);
					}
					break;

				case "heartbeatResponse":
					{
						lastHeartbeatChecked = DateTime.Now;
					}
					break;

				default:
					break;
			}
		}

		public void Dispose()
		{
			ctsClose.Dispose();
			listener?.Stop();
		}
	}
}