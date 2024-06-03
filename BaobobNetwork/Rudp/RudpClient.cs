namespace BaobabNetwork
{
	using System.Net;
	using System.Net.Sockets;
	using System.Text;

	public class RudpClient
	{
		private UdpClient udpClient;
		private IPEndPoint remoteEndPoint;
		private int sequenceNumber = 0;
		private int maxRetransmissions = 5;  // 최대 재전송 횟수
		private int retransmissions = 0;    // 현재 재전송 횟수
		private int maxBufferSize = 1024; // 최대 버퍼 크기

		public RudpClient(string serverIp, int serverPort)
		{
			udpClient = new UdpClient();
			remoteEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
		}

		public async Task SendAsync(string message)
		{
			byte[] data = Encoding.UTF8.GetBytes(message);
			RudpPacket packet = new RudpPacket { SequenceNumber = sequenceNumber, Data = data };

			while (retransmissions < maxRetransmissions)
			{
				if (udpClient.Available >= maxBufferSize)
				{
					Console.WriteLine("버퍼 크기 초과. 재전송 불가");
					throw new BaobabOverBufferSize("버퍼 크기 초과");
				}

				byte[] packetBytes = packet.ToBytes();
				await udpClient.SendAsync(packetBytes, packetBytes.Length, remoteEndPoint);

				using (var cts = new CancellationTokenSource(1000))
				{
					try
					{
						UdpReceiveResult result = await udpClient.ReceiveAsync().WithCancellation(cts.Token);
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
						Console.WriteLine($"타임아웃, 패킷 재전송 {retransmissions}");
						retransmissions++;
					}
				}
			}
			if (retransmissions >= maxRetransmissions)
			{
				throw new BaobabMaxRetryTransmission("Over max retransmission counts");
			}
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