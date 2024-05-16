namespace Server
{
	public class ParallelSingleWorker
	{
		private ConcurrentExclusiveSchedulerPair ConcurrentExclusiveSchedulerPair;

		private TaskFactory factory;

		private CancellationToken CancellationToken;

		/// <summary>
		/// 동시에 여러개의 작업이 실행이 가능하다.
		/// </summary>
		public ParallelSingleWorker()
		{
			this.factory = new TaskFactory(new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 8).ConcurrentScheduler);
		}

		public Task TryQueue(Action action)
		{
			return factory.StartNew(action);
		}
	}

	/// <summary>
	/// 동시에 하나의 작업만 실행할 수 있도록 보장한다.
	/// </summary>
	public class SequentialSingleWorker
	{
		private ConcurrentExclusiveSchedulerPair ConcurrentExclusiveSchedulerPair;
		private TaskFactory factory;
		private CancellationToken CancellationToken;

		public SequentialSingleWorker()
		{
			this.factory = new TaskFactory(new ConcurrentExclusiveSchedulerPair().ExclusiveScheduler);
		}

		public Task TryQueue(Action action)
		{
			return factory.StartNew(action);
		}
	}
}