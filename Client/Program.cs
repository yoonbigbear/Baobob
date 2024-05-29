namespace Client
{
	using System.Net;
	using System.Net.Sockets;

	internal class Program
	{
		private static async Task Main(string[] args)
		{
			int retryCount = 0;

			NetworkController networkSession = new NetworkController();
			while (!networkSession.Connected)
			{
				Console.WriteLine("Try Coonect to Server");
				try
				{
					await networkSession.ConnectAsync(IPAddress.Parse("127.0.0.1"), 8888);
					await Task.Delay(1000);
				}
				catch (SocketException ex)
				{
					Console.WriteLine($"Connection Failed. Retry attemp {retryCount += 1}");
				}
			}

			while (true)
			{
				Thread.Sleep(1000);
			}
		}
	}
}