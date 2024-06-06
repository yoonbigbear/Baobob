namespace Client
{
	using BaobabDispatcher;
	using Google.FlatBuffers;
	using MyGame.Sample;

	public class MessageHandler : HandlerDispatcher<IFlatbufferObject, PacketId>
	{
		[BaobabDispatcher]
		public static void Vec3Handler(Vec3 message)
		{
			Console.WriteLine($"{message}");
		}

		[BaobabDispatcher]
		public static async Task PacketHandler(Packet message)
		{
			await Task.Yield();
			Console.WriteLine($"{message.Message}");
		}
	}
}