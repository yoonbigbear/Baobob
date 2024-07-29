namespace BaobabNetwork
{
	using BaobabNetwork.Udp;
	using System;
	using System.Collections.Concurrent;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// 패킷 손실에 따른 timeout은 ConcurrentDictionary를 사용하는 방식으로 변경하자.
	/// </summary>
	public class RudpClient : UdpClient
	{
		private IPEndPoint remoteEndPoint;
		private int sequenceNumber = 0;
		private int maxRetransmissions = 5;  // 최대 재전송 횟수
		private int retransmissions = 0;    // 현재 재전송 횟수
		private int maxBufferSize = 1024; // 최대 버퍼 크기
		private static readonly int timeoutInterval = 1000; // 타임아웃 시간 (1초)

		public RudpClient(string serverIp, int serverPort)
		{
			remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
		}

		public async Task SendAsync(byte[] buffer)
		{
			RudpPacket packet = new RudpPacket
			{
				Header = new RudpHeader
				{
					SequenceNumber = sequenceNumber,
					Checksum = RudpPacket.CalculateChecksum(buffer)
				},
				Data = buffer
			};

			while (retransmissions < maxRetransmissions)
			{
				if (Available >= maxBufferSize)
				{
					Console.WriteLine("버퍼 크기 초과. 재전송 불가");
					throw new BaobabOverBufferSize("버퍼 크기 초과");
				}

				byte[] packetBytes = packet.Serialize();
				await SendAsync(packetBytes, packetBytes.Length, remoteEndPoint);

				using (var cts = new CancellationTokenSource(timeoutInterval))
				{
					try
					{
						UdpReceiveResult result = await ReceiveAsync().WithCancellation(cts.Token);
						int ackSequenceNumber = BitConverter.ToInt32(result.Buffer, 0);

						if (ackSequenceNumber == sequenceNumber)
						{
							Console.WriteLine($"ACK 수신됨: {ackSequenceNumber}");
							sequenceNumber++;
							retransmissions = 0;
							break;
						}
					}
					catch (OperationCanceledException)
					{
						// 손실된 패킷 재전송
						Console.WriteLine($"타임아웃, 패킷 재전송 {retransmissions}");
						await SendAsync(packetBytes, packetBytes.Length, remoteEndPoint);
						retransmissions++;
					}
				}
			}
			if (retransmissions >= maxRetransmissions)
			{
				throw new BaobabMaxRetryTransmission("Over max retransmission counts");
			}
		}

		public async Task SendAsync(string message)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(message);
			await SendAsync(buffer);
		}
	}

	public static class TaskExtensions
	{
		public static async Task<TResult> WithCancellation<TResult>(this Task<TResult> task, CancellationToken cancellationToken)
		{
			using (var delayCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
			{
				var delayTask = Task.Delay(Timeout.Infinite, delayCts.Token);
				var completedTask = await Task.WhenAny(task, delayTask);

				if (completedTask == delayTask)
				{
					throw new OperationCanceledException(cancellationToken);
				}

				delayCts.Cancel();
				return await task;
			}
		}
	}
}