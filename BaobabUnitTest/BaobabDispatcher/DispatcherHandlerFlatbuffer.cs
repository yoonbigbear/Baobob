using Google.FlatBuffers;
using UnitTest;

namespace BaobabDispatcher
{
	public class DispatcherHandlersIFlatbuffer<T> : HandlerDispatcher<T>
	{
		public string? Message1 { get; set; }

		public string? Message2 { get; set; }

		[BaobabDispatcher]
		private void HandlePacket(Packet message)
		{
			var packet = UnitTest.Packet.GetRootAsPacket(message.ByteBuffer);
			Message1 = packet.Name;
		}

		[BaobabDispatcher]
		private async Task AsyncHandlePacket(Packet message)
		{
			await Task.Yield();
			var packet = UnitTest.Packet.GetRootAsPacket(message.ByteBuffer);
			Message2 = packet.Name;
		}
	}
}