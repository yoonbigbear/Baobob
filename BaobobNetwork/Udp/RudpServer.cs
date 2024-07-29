namespace BaobabNetwork
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading.Tasks;

	public class RudpServer
	{
		private UdpClient udpClient;
		private int expectedSequenceNumber = 0;
		private bool isCongested = false;

		//버퍼 크기 상한값
		private const int MaxBufferSize = 1024;

		public RudpServer(int port)
		{
			udpClient = new UdpClient(port);
		}

		public async Task StartAsync()
		{
			Console.WriteLine("RUDP 서버 시작됨...");

			while (true)
			{
				// 클라이언트로부터 UDP 패킷 수신
				UdpReceiveResult result = await udpClient.ReceiveAsync();
				ProcessReceivedPacket(result);
			}
		}

		private async void SendAck(byte[] ack, IPEndPoint remoteEndPoint)
		{
			await udpClient.SendAsync(ack, ack.Length, remoteEndPoint);
		}

		private void ProcessReceivedPacket(UdpReceiveResult result)
		{
			// 클라이언트로부터 UDP 패킷 수신
			RudpPacket packet = RudpPacket.Deserialize(result.Buffer);

			// 순서 번호가 예상된 번호와 일치하는지 확인
			if (packet.Header.SequenceNumber == expectedSequenceNumber)
			{
				// 패킷 내용 출력
				Console.WriteLine($"수신된 패킷: {packet.Header.SequenceNumber} - {Encoding.UTF8.GetString(packet.Data!)}");
				expectedSequenceNumber++;
			}
			else if (packet.Header.SequenceNumber > expectedSequenceNumber)
			{
				if (isCongested)
				{
					Console.WriteLine("혼잡 상태에서는 ACK만 전송`");
					SendAck(BitConverter.GetBytes(expectedSequenceNumber), result.RemoteEndPoint);
					return;
				}

				if (udpClient.Available >= MaxBufferSize)
					Console.WriteLine($"수신된 패킷: {packet.Header.SequenceNumber} - 대기중인 패킷 {expectedSequenceNumber}");
				SendAck(BitConverter.GetBytes(expectedSequenceNumber), result.RemoteEndPoint);
			}

			// ACK 전송: 클라이언트의 IPEndPoint를 사용하여 ACK 전송
			SendAck(BitConverter.GetBytes(packet.Header.SequenceNumber), result.RemoteEndPoint);
		}
	}
}