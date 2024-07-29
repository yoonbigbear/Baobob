namespace Client;

using BaobabNetwork;
using BaobabNetwork.Tcp;
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
				Console.WriteLine($"Connection Failed. Retry attemp {retryCount += 1} {ex.Message}");
			}
		}

		RudpClient client = new RudpClient("127.0.0.1", 9000);
		for (int i = 0; i < 210; ++i)
		{
			//의도적으로 패킷을 드랍시켜서 수신측에서 재전송을 요청하도록 합니다.
			//if (i == 2 || Random.Shared.Next(0, 2) == 0)
			//{
			//	Console.WriteLine($"Dropping Packet {i}");
			//	continue;
			//}
			await client.SendAsync($"message {i}");
		}

		while (true)
		{
			var builder = new FlatBufferBuilder(128);
			var buf = builder.SerializePacket("Hello?");
			if (networkSession.Connected)
			{
				await networkSession.SendAsync(buf);
			}
			await Task.Delay(1000);
		}
	}
}

public static partial class FlatBufferExtentions
{
	public static byte[] SerializePacket(this FlatBufferBuilder fb, string message)
	{
		var offset = MyGame.Sample.Packet.CreatePacket(fb, fb.CreateSharedString(message));
		fb.Finish(offset.Value);
		return TcpPayload.Serialize((int)PacketId.Packet, fb.SizedByteArray());
	}
}