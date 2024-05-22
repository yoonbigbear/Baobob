using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace BaobobDispatcher
{
	public class HandlerNotFoundException : Exception
	{
	}

	public class HandlerNotImplementedException : Exception
	{
	}

	public interface IRootMessage
	{
	}

	public interface ICaller
	{
		public void Handle(IRootMessage message);
	}

	public interface IAsyncCaller : ICaller
	{
		public new void Handle(IRootMessage message);
	}

	public class Caller : ICaller
	{
		public void Handle(IRootMessage message)
		{
			throw new HandlerNotImplementedException();
		}
	}

	public class AsyncCaller : IAsyncCaller
	{
		public virtual async void Handle(IRootMessage message)
		{
			await Task.CompletedTask;
			throw new HandlerNotImplementedException();
		}
	}

	public class PacketDispatcher
	{
		private ImmutableDictionary<int, ICaller> packetHandler = ImmutableDictionary<int, ICaller>.Empty;

		public PacketDispatcher()
		{ }

		public PacketDispatcher(Dictionary<int, ICaller> handlers)
		{
			this.packetHandler.AddRange(handlers);
		}

		public void Invoke(int key, IRootMessage message)
		{
			if (!this.packetHandler.ContainsKey(key))
			{
				throw new HandlerNotFoundException();
			}
		}
	}
}