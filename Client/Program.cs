namespace Client;

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
				await networkSession.ConnectAsync(IPAddress.Parse("127.0.0.1"), 8888).ConfigureAwait(false);
				await Task.Delay(1000).ConfigureAwait(false);
			}
			catch (SocketException ex)
			{
				Console.WriteLine($"Connection Failed. Retry attemp {retryCount += 1} {ex.Message}");
			}
		}

		while (true)
		{
			var builder = new FlatBufferBuilder(128);
			var buf = builder.SerializePacket("Hello?");
			if (networkSession.Connected)
			{
				await networkSession.SendAsync(buf).ConfigureAwait(false);
			}
			await Task.Delay(1000).ConfigureAwait(false);
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