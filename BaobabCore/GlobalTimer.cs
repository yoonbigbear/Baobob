namespace BaobobCore
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	public class GlobalTimer
	{
		public static float Delta = 0.0f;
		public static float DeltaMs = 0.0f;

		public static double Rtt = 0.0f;

		//ServerTime
		public static long UnixServeTime = 0;

		//time delta
		private double AccumulatedMs = 0;

		private long start;
		private long now;

		//time sync interval
		private static long _lastRttSend;

		//desired frame rate
		private float _fpsToSecond = 0;

		public void Start()
		{
			start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}

		public void DesiredFps(int fps)
		{
			if (fps <= 0)
				return;
			_fpsToSecond = 1000 / fps;
		}

		private void UpdateDeltaTime()
		{
			now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			var interval = now - start;
			start = now;

			AccumulatedMs += (interval);
		}

		public bool UpdateDelta()
		{
			UpdateDeltaTime();
			if (AccumulatedMs > _fpsToSecond)
			{
				Delta = (float)(AccumulatedMs * 0.001f);
				DeltaMs = (float)AccumulatedMs;
				AccumulatedMs = 0;
				return true;
			}
			return false;
		}

		public static void SendRtt()
		{
			_lastRttSend = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		}

		public static void RecvRtt()
		{
			var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			Rtt = (now - _lastRttSend) >> 1;
			_lastRttSend = now;
		}

		public static void Repeat<T>(TimeSpan timeSpan, Func<T> func, CancellationToken cancellationToken)
		{
			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					Task.Delay(timeSpan);
					func.Invoke();
				}
			}
			, cancellationToken);
		}
	}
}