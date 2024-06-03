namespace Client
{
	using BaobabNetwork;
	using System.Buffers;
	using System.Net.Sockets;

	internal class NetworkController : ClientBuilder
	{
		public override void AcceptSession(Socket? socket)
		{
			base.AcceptSession(socket);
			Console.WriteLine($"Connect success");
		}

		public async Task SendAsync(ReadOnlyMemory<byte> readOnlyMemory)
		{
			await tcpClient!.GetStream().WriteAsync(readOnlyMemory);
		}

		public async void ReadAsync()
		{
			var array = ArrayPool<byte>.Shared.Rent(4096);
			await tcpClient!.GetStream().ReadAsync(array);
		}
	}
}