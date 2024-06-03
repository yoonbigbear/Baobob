namespace Server
{
	using BaobabDispatcher;
	using BaobobCore;
	using Google.FlatBuffers;
	using MyGame.Sample;

	public partial class MessageHandler : HandlerDispatcher<IFlatbufferObject, PacketId>
	{
		[BaobabDispatcher]
		public static void Vec3Handler(Vec3 message)
		{
			Logger.Trace($"{message}");
		}

		[BaobabDispatcher]
		public static async Task PacketHandler(Packet message)
		{
			await Task.Yield();
			Logger.Trace($"{message}");
		}
	}
}