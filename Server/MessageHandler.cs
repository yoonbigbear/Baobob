namespace Server
{
	using Google.FlatBuffers;
	using BaobabDispatcher;
	using MyGame.Sample;
	using BaobobCore;

	public partial class MessageHandler : HandlerDispatcher<IFlatbufferObject>
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