namespace BaobabNetwork
{
	public class BaobabNetworkException : Exception
	{
		public BaobabNetworkException()
		{ }

		public BaobabNetworkException(string? message) : base(message)
		{
		}
	}

	public class BaobabMaxRetryTransmission : Exception
	{
		public BaobabMaxRetryTransmission()
		{ }

		public BaobabMaxRetryTransmission(string? message) : base(message)
		{
		}
	}

	public class BaobabInvalidChecksum : Exception
	{
		public BaobabInvalidChecksum()
		{ }

		public BaobabInvalidChecksum(string? message) : base(message)
		{
		}
	}

	public class BaobabOverBufferSize : Exception
	{
		public BaobabOverBufferSize()
		{
		}

		public BaobabOverBufferSize(string? message) : base(message)
		{
		}
	}
}