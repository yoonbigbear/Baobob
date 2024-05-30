namespace BaobabDispatcher
{
	using System.Collections.Immutable;

	public abstract partial class HandlerDispatcher<T>
	{
		public static ImmutableDictionary<int, ICaller<T>> MessageHandler { get; private set; } = ImmutableDictionary<int, ICaller<T>>.Empty;

		public static int Count { get => MessageHandler.Count; }

		public static async Task Invoke(int id, T message)
		{
			if (!MessageHandler.TryGetValue(id, out ICaller<T>? caller))
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

		public static bool IsAsyncCall(int id, T message)
		{
			if (!MessageHandler.TryGetValue(id, out ICaller<T>? caller))
			{
				throw new HandlerNotFoundException();
			}
			return caller is IAsyncCaller<T>;
		}
	}
}