namespace Project
{
	using System.Net;
	using System.Text;

	internal class Program
	{
		private static async Task Main(string[] args)
		{
			string Ip = "127.0.0.1";
			int Port = 11000;

			System.Net.Sockets.TcpListener tcpListener = new(IPAddress.Parse(Ip), Port);
			tcpListener.Start();

			//Session
			{
				Console.WriteLine($"Accept Client");
				await Listen(tcpListener);
			}

			while (true)
			{
				await Task.Delay(3000);
			}
		}

		private static async Task Listen(System.Net.Sockets.TcpListener tcpListener)
		{
			var socket = await tcpListener.AcceptSocketAsync();

			byte[] bytes = new byte[1024];

			await socket.SendAsync(Encoding.UTF8.GetBytes("Hello"));
		}
	}
}