namespace Client
{
	using BaobabNetwork;
	using Google.FlatBuffers;
	using MyGame.Sample;
	using System.Net;
	using System.Net.Sockets;
	using System.Reflection;

	internal class Program
	{
		private static async Task Main(string[] args)
		{
			MessageHandler.BindHandler(Assembly.GetExecutingAssembly());

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

			RudpClient client = new RudpClient("127.0.0.1", 9000);
			for (int i = 0; i < 210; ++i)
			{
				await client.SendAsync($"message {i}");
			}

			while (true)
			{
				var builder = new FlatBufferBuilder(128);
				var offset = MyGame.Sample.Packet.CreatePacket(builder, builder.CreateSharedString("Hello?"));
				builder.Finish(offset.Value);
				var buf = Payload.Serialize((int)PacketId.Packet, builder.SizedByteArray());
				if (networkSession.Connected)
				{
					await networkSession.SendAsync(buf);
				}
				Thread.Sleep(1000);
			}
		}
	}
}