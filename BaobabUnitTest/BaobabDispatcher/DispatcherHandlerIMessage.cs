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

	public class DispatcherHandlersIMessage : HandlerDispatcher<IMessage>
	{
		public string? Message1 { get; set; }

		public string? Message2 { get; set; }

		[BaobabDispatcher]
		public void HandlePacket(Message1 message)
		{
			Message1 = message.Message;
		}

		[BaobabDispatcher]
		public async Task AsyncHandlePacket(Message2 message)
		{
			await Task.Yield();
			Message2 = message.Message;
		}
	}
}