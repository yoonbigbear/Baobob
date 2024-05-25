namespace BaobabDispatcher
{
	public interface IMessage
	{
		int MessageID { get; }
	}

	public interface ICaller
	{
		public void Invoke(IMessage message);
	}

	public interface IAsyncCaller : ICaller
	{
		public new Task Invoke(IMessage message);
	}

	public class Caller : ICaller
	{
		private Action<IMessage> action;

		public Caller(Action<IMessage> action)
		{
			this.action = action;
		}

		public void Invoke(IMessage message) => this.action(message);
	}

	public class AsyncCaller : IAsyncCaller
	{
		private Func<IMessage, Task> func;

		public AsyncCaller(Func<IMessage, Task> func)
		{
			this.func = func;
		}

		public virtual async Task Invoke(IMessage message)
		{
			Task asyncFunc = func.Invoke(message);
			if (asyncFunc == null)
			{
				throw new HandlerNotImplementedException();
			}
			await asyncFunc;
		}

		void ICaller.Invoke(IMessage message) => throw new NotImplementedException();
	}
}