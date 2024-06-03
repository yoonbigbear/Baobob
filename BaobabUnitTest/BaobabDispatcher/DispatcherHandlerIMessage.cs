namespace BaobabUnitTest
{
	using BaobabDispatcher;

	public record Message1 : IMessage
	{
		public int MessageID { get; } = 1;

		public string? Message { get; set; }
	}

	public record Message2 : IMessage
	{
		public int MessageID { get; } = 2;

		public string? Message { get; set; }
	}

	public class DispatcherHandlersIMessage : HandlerDispatcher<IMessage, int>
	{
		public static string? Message1 { get; set; }

		public static string? Message2 { get; set; }

		[BaobabDispatcher]
		public static void HandlePacket(Message1 message)
		{
			Message1 = message.Message;
		}

		[BaobabDispatcher]
		public static async Task AsyncHandlePacket(Message2 message)
		{
			await Task.Yield();
			Message2 = message.Message;
		}
	}
}