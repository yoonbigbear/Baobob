namespace Server
{
	public class ParallelSingleWorker
	{
		private ConcurrentExclusiveSchedulerPair ConcurrentExclusiveSchedulerPair;

		private TaskFactory factory;

		private CancellationToken CancellationToken;

		public ParallelSingleWorker()
		{
			this.factory = new TaskFactory(new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 10).ConcurrentScheduler);
		}

		public Task TryQueue(Action action)
		{
			return factory.StartNew(action);
		}
	}

	public class SequentialSingleWorker
	{
		private ConcurrentExclusiveSchedulerPair ConcurrentExclusiveSchedulerPair;
		private TaskFactory factory;
		private CancellationToken CancellationToken;

		public SequentialSingleWorker()
		{
			this.factory = new TaskFactory(new ConcurrentExclusiveSchedulerPair(TaskScheduler.Default, 1).ExclusiveScheduler);
		}

		public Task TryQueue(Action action)
		{
			return factory.StartNew(action);
		}
	}
}