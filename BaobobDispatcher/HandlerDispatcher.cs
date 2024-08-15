namespace BaobabDispatcher
{
#if NET5_0_OR_GREATER

	using System.Collections.Immutable;

#else
	using System.Collections.Concurrent;
#endif

	using System.Threading.Tasks;

	public abstract partial class HandlerDispatcher<T, EnumType>
	{
#if NET5_0_OR_GREATER
		public static ImmutableDictionary<int, ICaller<T>> MessageHandler { get; private set; } = ImmutableDictionary<int, ICaller<T>>.Empty;
#else
		public static ConcurrentDictionary<int, ICaller<T>> MessageHandler { get; private set; } = new ConcurrentDictionary<int, ICaller<T>>();
#endif
		public static int Count { get => MessageHandler.Count; }

		public static async Task Invoke(int id, T message)
		{
			if (!MessageHandler.TryGetValue(id, out ICaller<T>? caller))
			{
				throw new HandlerNotFoundException();
			}
			if (caller is IAsyncCaller<T> asyncCaller)
			{
				await asyncCaller.Invoke(message).ConfigureAwait(false);
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