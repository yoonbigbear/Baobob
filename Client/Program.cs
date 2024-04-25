namespace Client
{
	using System.Text;

	internal class Program
	{
		private static async Task Main(string[] args)
		{
			string Ip = "127.0.0.1";
			int Port = 11000;
			System.Net.Sockets.TcpClient tcpClient = new();

			tcpClient.ConnectAsync(Ip, Port).Wait();
			if (tcpClient.Connected)
			{
				Console.WriteLine($"Server Connected");
			}

			while (true)
			{
				await Task.Delay(3000);
				byte[] b = new byte[1024];
				Memory<byte> buffer = new Memory<byte>(b);
				var size = await tcpClient.Client.ReceiveAsync(buffer);
				var str = Encoding.Default.GetString(buffer.Slice(0, size).Span);

				Console.WriteLine(str);
			}
		}
	}
}