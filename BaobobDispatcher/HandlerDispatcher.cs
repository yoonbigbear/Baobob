namespace BaobabDispatcher
{
	using System.Collections.Immutable;

	public abstract partial class HandlerDispatcher<T>
	{
		private ImmutableDictionary<int, ICaller<T>> packetHandler = ImmutableDictionary<int, ICaller<T>>.Empty;

		public int Count { get => packetHandler.Count; }

		public async Task Invoke(int id, T message)
		{
			if (!this.packetHandler.TryGetValue(id, out ICaller<T>? caller))
			{
				throw new HandlerNotFoundException();
			}
			if (caller is IAsyncCaller<T> asyncCaller)
			{
				await asyncCaller.Invoke(message);
			}
			else
			{
				caller.Invoke(message);
			}
		}

		public bool IsAsyncCall(int id, T message)
		{
			if (!this.packetHandler.TryGetValue(id, out ICaller<T>? caller))
			{
				throw new HandlerNotFoundException();
			}
			return caller is IAsyncCaller<T>;
		}
	}
}