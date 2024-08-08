namespace BaobabNetwork
{
	using BaobabNetwork.Tcp;
	using BaobobCore;
	using System;
	using System.Runtime.InteropServices;
	using System.Threading.Tasks;

	public partial class TcpSession
	{
		protected enum HeartbeatProtocol
		{
			Knock = -100,
			Response = -99,
			TimeRequest = -98,
		}

		private DateTime sentTime { get; set; } = DateTime.MinValue;
		private DateTime clientNow { get; set; } = DateTime.MinValue;
		private TimeSpan rtt { get; set; }
		private TimeSpan heartbeatInterval { get; set; }
		private TimeSpan heartbeatTimeout { get; set; }

		/// <summary>
		/// 서버에서 heartbeatInterval 마다 클라이언트에게 HearbeatKnock을 보냅니다.
		/// </summary>
		protected void RepeatKnock()
		{
			if (heartbeatInterval >= heartbeatTimeout)
			{
				Logger.Error($"Heartbeat timeout is too low interval: {heartbeatInterval}, timeout: {heartbeatTimeout}");
				return;
			}

			if (heartbeatInterval.TotalSeconds > 2)
			{
				var task = Task.Run(async () =>
				{
					while (true)
					{
						sentTime = DateTime.UtcNow;
						await tcpStream.WriteAsync(TcpPayload.Serialize((int)HeartbeatProtocol.Knock, BitConverter.GetBytes(sentTime.Ticks)));
						await Task.Delay(heartbeatInterval);
					}
				}, cancellationTokenSource.Token);
			}
			else
			{
				Logger.Error($"Heartbeat interval is to low : {heartbeatInterval}");
			}
		}

		/// <summary>
		/// 클라이언트에서 Timeout을 주기적으로 확인합니다.
		/// </summary>
		protected void RepeatCheckTimeout()
		{
			if (heartbeatInterval.TotalSeconds > 2)
			{
				var task = Task.Run(async () =>
				{
					while (true)
					{
						await Task.Delay(heartbeatInterval);
						var now = DateTime.UtcNow;
						var timeSpent = now - sentTime;
						if (timeSpent > heartbeatTimeout)
						{
							OnHeartBeatTimeout();
						}
					}
				}, cancellationTokenSource.Token);
			}
			else
			{
				Logger.Error($"Heartbeat interval is to low : {heartbeatInterval}");
			}
		}

		/// <summary>
		/// 서버로부터 HeartbeatKnock을 받아 Knock을 보냅니다.
		/// </summary>
		protected async Task ResponseKnockFromServer()
		{
			sentTime = DateTime.UtcNow;
			await tcpStream.WriteAsync(TcpPayload.Serialize((int)HeartbeatProtocol.Knock, BitConverter.GetBytes(sentTime.Ticks)));
		}

		/// <summary>
		/// Response를 받으면 다시 Response를 응답하면서 RTT를 계산합니다.
		/// </summary>
		protected async Task CalculateRTT(long sentTime)
		{
			var now = DateTime.UtcNow;
			var displacement = now.Ticks - sentTime;
			if (displacement > heartbeatTimeout.Ticks)
			{
				OnHeartBeatTimeout();
				return;
			}
			rtt = TimeSpan.FromTicks(displacement >> 1);
			Logger.Trace($"RTT : {rtt.TotalMilliseconds}");
			await WriteAsync(TcpPayload.Serialize((int)HeartbeatProtocol.Response, BitConverter.GetBytes(now.Ticks)));
		}

		/// <summary>
		/// Timeout이 감지될 경우 호출되어 연결을 끊습니다.
		/// </summary>
		protected virtual void OnHeartBeatTimeout()
		{
			if (sentTime == DateTime.MinValue)
			{
				sentTime = DateTime.UtcNow;
				return;
			}

			Logger.LogWarning($"Session heartbeat timeout Id:{SessionId}, Interval:{heartbeatInterval}");
			tcpStream.Close();
		}

		/// <summary>
		/// 서버의 현재 시간을 보냅니다.
		/// </summary>
		/// <param name="sentTime"></param>
		protected async Task TimeRequest()
		{
			clientNow = DateTime.UtcNow;
			await WriteAsync(TcpPayload.Serialize((int)HeartbeatProtocol.TimeRequest, BitConverter.GetBytes(clientNow.Ticks)));
		}

		protected void EstimateServerTime(long serverTime)
		{
			long roundTripTime = (DateTime.UtcNow - clientNow).Ticks;
			long estimatedServerTime = serverTime + (roundTripTime >> 2);

			Logger.Trace($"Server Time : {serverTime}");
			Logger.Trace($"estimate Server Time : {estimatedServerTime}");
			DateTime serverDateTime = new DateTime(estimatedServerTime);
			Logger.Trace("서버 시간: " + serverDateTime);
		}
	}
}