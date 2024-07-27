namespace BaobabNetwork
{
	using System;

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

	public class BaobabInvalidRudpHeader : Exception
	{
		public BaobabInvalidRudpHeader()
		{ }

		public BaobabInvalidRudpHeader(string? message) : base(message)
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