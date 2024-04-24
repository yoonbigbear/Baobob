namespace Project
{
	using System.Net;

	internal class Program
	{
		private static void Main(string[] args)
		{
			string Ip = "127.0.0.1";
			int Port = 11000;

			System.Net.Sockets.TcpListener tcpListener = new(IPAddress.Parse(Ip), Port);
			tcpListener.Start();

			tcpListener.AcceptSocketAsync().Wait();

			Console.WriteLine($"Accept Client");
			while (true)
			{
				Thread.Sleep(3000);
			}
		}
	}
}