namespace Client
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			string Ip = "127.0.0.1";
			int Port = 11000;
			System.Net.Sockets.TcpClient tcpClient = new();

			tcpClient.ConnectAsync(Ip, Port).Wait();
			if (tcpClient.Connected)
			{
				Console.WriteLine($"Server Connected");
			}
		}
	}
}