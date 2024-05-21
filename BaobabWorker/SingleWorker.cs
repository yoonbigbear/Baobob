namespace BaobabWorker
{
	/// <summary>
	/// 동시에 하나의 작업만 실행할 수 있도록 보장한다.
	/// </summary>
	public class SingleWorker
	{
		private TaskFactory factory;

		public SingleWorker()
		{
			this.factory = new TaskFactory(new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);
		}

		public virtual Task TryQueue(Action action)
		{
			return factory.StartNew(action);
		}
	}
}