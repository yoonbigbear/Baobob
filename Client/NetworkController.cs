namespace Client
{
	using BaobabNetwork;
	using System;
	using System.Net.Sockets;

	internal class NetworkController : ClientBuilder
	{
		private NetworkSession? session { get; set; }

		public override void AcceptSession(Socket? socket)
		{
			base.AcceptSession(socket);
			Console.WriteLine($"Connect success");

			session = new NetworkSession(tcpClient!.Client, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(4));
		}

		public async Task SendAsync(ReadOnlyMemory<byte> readOnlyMemory) => await session!.SendAsync(readOnlyMemory);
	}
}