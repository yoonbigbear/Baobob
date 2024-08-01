using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BaobabP2P
{
	/* STUN
		STUN(Session Traversal Utilities for NAT)은 NAT Traversal을 돕는 주요 기술
		서버는 클라이언트가 자신의 공용 IP 주소와 포트를 알아내고, 이를 기반으로 P2P 연결
		STUN은 자신의 공용 IP와 공용 Port를 알아내고 클라이언트가 어떤 유형의 NAT를 사용하는지 확인한다.
		그리고 클라이언트간의 P2P 연결을 지원하기 위해 필요한 정보를 제공한다.

		 > NAT는 공인 IP를 사설 IP로 변환하는 기술이다.

		클라이언트는 STUN 서버에 자신의 공용 IP와 포트를 요청한다.
		STUN서버는 클라이언트에 정보 전달
		클라이언트는 이 정보를 통해 NAT 홀펀칭을 한다.
	*/

	public class Stun
	{
		private static readonly (string ip, int port)[] stunServerList = {
			("stun.l.google.com",19302),
			("stun1.l.google.com",19302),
			("stun2.l.google.com",19302),
			("stun3.l.google.com",19302),
			("stun4.l.google.com",19302),
		};

		public IPEndPoint? GetMyPulicIP()
		{
			var stunServer = stunServerList.FirstOrDefault();
			string stunIp = stunServer.ip;
			int stunPort = stunServer.port;

			UdpClient udpClient = new UdpClient();
			udpClient.Connect(stunIp, stunPort);

			// STUN 바인드 요청 생성
			byte[] stunRequest = CreateStunRequest();
			udpClient.Send(stunRequest, stunRequest.Length);

			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);

			// 응답 대기 및 수신
			byte[] stunResponse = udpClient.Receive(ref remoteEP);

			// STUN 응답 처리
			return HandleStunResponse(stunResponse);
		}

		private static byte[] CreateStunRequest()
		{
			// STUN 요청 메시지 생성 (기본 헤더만 포함)
			byte[] stunMessage = new byte[20];

			// 메시지 타입 (바인드 요청)
			stunMessage[0] = 0x00;
			stunMessage[1] = 0x01;

			// 메시지 길이 (0, 헤더만 포함)
			stunMessage[2] = 0x00;
			stunMessage[3] = 0x00;

			// 트랜잭션 ID (랜덤 값)
			Random rand = new Random();
			for (int i = 4; i < 20; i++)
			{
				stunMessage[i] = (byte)rand.Next(0, 256);
			}

			return stunMessage;
		}

		private static IPEndPoint? HandleStunResponse(byte[] response)
		{
			if (response.Length < 20)
			{
				Console.WriteLine("Invalid STUN response.");
				return null;
			}

			// 응답 메시지 타입 확인 (바인드 응답)
			if (response[0] == 0x01 && response[1] == 0x01)
			{
				int index = 20;
				while (index < response.Length)
				{
					// 속성 타입 (2 바이트)
					ushort type = (ushort)((response[index] << 8) | response[index + 1]);
					// 속성 길이 (2 바이트)
					ushort length = (ushort)((response[index + 2] << 8) | response[index + 3]);

					if (type == 0x0001) // MAPPED-ADDRESS
					{
						// 주소 패밀리 (1 바이트)
						byte addressFamily = response[index + 5];

						// 포트 (2 바이트, big-endian)
						// NAT 장치에서 공용 네트워크와 통신할 때 사용되는 포트 번호.
						int port = (response[index + 6] << 8) | response[index + 7];

						// IP 주소 (4 바이트, IPv4의 경우)
						// NAT 장치를 지나서 공용 네트워크에서 클라이언트가 사용하는 IP 주소.
						string ip = $"{response[index + 8]}.{response[index + 9]}.{response[index + 10]}.{response[index + 11]}";

						// 이 공용 포트는 다른 클라이언트 피어가 P2P 연결을 시도할 때 사용합니다.
						Console.WriteLine($"공용 IP: {ip}, 공용 포트: {port}");
						return new IPEndPoint(IPAddress.Parse(ip), port);
					}

					index += (4 + length);
				}

				Console.WriteLine("MAPPED-ADDRESS 속성을 찾을 수 없습니다.");
			}
			else
			{
				Console.WriteLine("Unexpected STUN response type.");
			}
			return null;
		}

		public static async Task<UdpClient> HolePunching(IPEndPoint peerEndPoint)
		{
			// NAT 홀 펀칭
			UdpClient peerClient = new UdpClient(peerEndPoint);

			// 초기 패킷 전송 (NAT에 구멍 뚫기)
			byte[] punchPacket = Encoding.UTF8.GetBytes("Punching a hole!");
			await peerClient.SendAsync(punchPacket, punchPacket.Length, peerEndPoint);

			Console.WriteLine("초기 패킷 전송 완료, 연결 시도 중...");

			// 데이터 수신 대기
			var data = await peerClient.ReceiveAsync();
			string receivedMessage = Encoding.UTF8.GetString(data.Buffer);
			Console.WriteLine($"받은 메시지: {receivedMessage}");
			return peerClient;
		}
	}
}