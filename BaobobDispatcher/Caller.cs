namespace BaobabDispatcher
{
	using System;
	using System.Threading.Tasks;

	public interface IMessage
	{
		int MessageID { get; }
	}

	public interface ICaller<T>
	{
		public void Invoke(T message);
	}

	public interface IAsyncCaller<T> : ICaller<T>
	{
		public new Task Invoke(T message);
	}

	public class Caller<T> : ICaller<T>
	{
		private Action<T> action;

		public Caller(Action<T> action)
		{
			this.action = action;
		}

		public void Invoke(T message) => this.action(message);
	}

	public class AsyncCaller<T> : IAsyncCaller<T>
	{
		private Func<T, Task> func;

		public AsyncCaller(Func<T, Task> func)
		{
			this.func = func;
		}

		public virtual async Task Invoke(T message)
		{
			Task asyncFunc = func.Invoke(message);
			if (asyncFunc == null)
			{
				throw new HandlerNotImplementedException();
			}
			await asyncFunc;
		}

		void ICaller<T>.Invoke(T message) => throw new NotImplementedException();
	}
}