using System.Net;
using System.Net.Sockets;
using System.Text;

namespace BaobabNetwork
{
	public class RUdpServer
	{
		public void RunUdpServer()
		{
			UdpClient udpClient = new UdpClient(18888);
			IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
			int expectedSeq = 0;

			Console.WriteLine("RUDP Server started...");

			while (true)
			{
				byte[] data = udpClient.Receive(ref clientEndPoint);
				string receivedMessage = Encoding.UTF8.GetString(data);
				string[] parts = receivedMessage.Split(':');
				int seqNum = int.Parse(parts[0]);
				string payload = parts[1];
				string checksum = parts[2];

				if (VerifyChecksum(payload, checksum) && seqNum == expectedSeq)
				{
					Console.WriteLine($"Received (seq {seqNum}): {payload}");
					expectedSeq++;
					SendAck(udpClient, clientEndPoint, seqNum);
				}
				else
				{
					Console.WriteLine($"Packet error or out of order (seq {seqNum})");
					SendNack(udpClient, clientEndPoint, expectedSeq);
				}
			}
		}

		private static void SendAck(UdpClient server, IPEndPoint clientEndPoint, int seqNum)
		{
			string ackMessage = $"ACK:{seqNum}";
			byte[] ackData = Encoding.UTF8.GetBytes(ackMessage);
			server.Send(ackData, ackData.Length, clientEndPoint);
			Console.WriteLine($"ACK {seqNum} sent");
		}

		private static void SendNack(UdpClient server, IPEndPoint clientEndPoint, int expectedSeq)
		{
			string nackMessage = $"NACK:{expectedSeq}";
			byte[] nackData = Encoding.UTF8.GetBytes(nackMessage);
			server.Send(nackData, nackData.Length, clientEndPoint);
			Console.WriteLine($"NACK {expectedSeq} sent");
		}

		private static bool VerifyChecksum(string payload, string checksum)
		{
			string calculatedChecksum = CalculateChecksum(payload);
			return calculatedChecksum == checksum;
		}

		private static string CalculateChecksum(string payload)
		{
			int checksum = 0;
			foreach (char c in payload)
			{
				checksum += c;
			}
			return checksum.ToString("X");
		}
	}

	public class RUdpClient
	{
		public void RunRUdpClient()
		{
			string serverIp = "127.0.0.1";
			UdpClient client = new UdpClient();
			client.Connect(serverIp, 18888);

			string message = "Hello from RUDP client";
			int seqNum = 0;
			byte[] data = CreatePacket(seqNum, message);

			IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);

			bool ackReceived = false;
			int retries = 0;
			const int maxRetries = 5;
			const int timeout = 1000; // milliseconds

			while (!ackReceived && retries < maxRetries)
			{
				try
				{
					// Send packet
					client.Send(data, data.Length);
					Console.WriteLine($"Packet {seqNum} sent");

					// Set timeout for receiving ACK/NACK
					client.Client.ReceiveTimeout = timeout;

					// Wait for ACK/NACK
					byte[] response = client.Receive(ref serverEndPoint);
					string responseMessage = Encoding.UTF8.GetString(response);
					string[] parts = responseMessage.Split(':');
					string responseType = parts[0];
					int responseSeq = int.Parse(parts[1]);

					if (responseType == "ACK" && responseSeq == seqNum)
					{
						ackReceived = true;
						Console.WriteLine($"ACK {seqNum} received");
					}
					else if (responseType == "NACK")
					{
						Console.WriteLine($"NACK {responseSeq} received, resending...");
						data = CreatePacket(responseSeq, message);
						retries++;
					}
				}
				catch (SocketException e)
				{
					if (e.SocketErrorCode == SocketError.TimedOut)
					{
						retries++;
						Console.WriteLine($"ACK not received, retrying... ({retries})");
					}
					else
					{
						throw;
					}
				}
			}

			if (!ackReceived)
			{
				Console.WriteLine($"Failed to receive ACK after {maxRetries} retries.");
			}
		}

		private static byte[] CreatePacket(int seqNum, string message)
		{
			string checksum = CalculateChecksum(message);
			string packet = $"{seqNum}:{message}:{checksum}";
			return Encoding.UTF8.GetBytes(packet);
		}

		private static string CalculateChecksum(string payload)
		{
			int checksum = 0;
			foreach (char c in payload)
			{
				checksum += c;
			}
			return checksum.ToString("X");
		}

		private record struct RUdpPacket
		{
			public uint ProtocolID { get; set; }
			public uint Sequence { get; set; }
			public uint Ack { get; set; }
			public uint AckBitField { get; set; }
			public byte[] Datas { get; set; }
		}
	}
}