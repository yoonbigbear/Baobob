namespace BaobabDispatcher
{
	using System.Collections.Immutable;

	public abstract partial class HandlerDispatcher
	{
		private ImmutableDictionary<int, ICaller> packetHandler = ImmutableDictionary<int, ICaller>.Empty;

		public int Count { get => packetHandler.Count; }

		public async Task Invoke(IMessage message)
		{
			if (!this.packetHandler.TryGetValue(message.MessageID, out ICaller? caller))
			{
				throw new HandlerNotFoundException();
			}
			if (caller is IAsyncCaller asyncCaller)
			{
				await asyncCaller.Invoke(message);
			}
			else
			{
				caller.Invoke(message);
			}
		}

		public bool IsAsyncCall(IMessage message)
		{
			if (!this.packetHandler.TryGetValue(message.MessageID, out ICaller? caller))
			{
				throw new HandlerNotFoundException();
			}
			return caller is IAsyncCaller;
		}
	}
}