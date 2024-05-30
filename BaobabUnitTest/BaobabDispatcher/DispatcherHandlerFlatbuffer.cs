using Google.FlatBuffers;
using UnitTest;

namespace BaobabDispatcher
{
	public class DispatcherHandlersIFlatbuffer : HandlerDispatcher<IFlatbufferObject>
	{
		public static string? Message1 { get; set; }

		public static string? Message2 { get; set; }

		[BaobabDispatcher]
		public static void HandlePacket(Packet1 message)
		{
			var packet = UnitTest.Packet1.GetRootAsPacket1(message.ByteBuffer);
			Message1 = packet.Name;
		}

		[BaobabDispatcher]
		public static async Task AsyncHandlePacket(Packet2 message)
		{
			await Task.Yield();
			var packet = UnitTest.Packet2.GetRootAsPacket2(message.ByteBuffer);
			Message2 = packet.Name;
		}
	}
}