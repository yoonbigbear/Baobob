using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BaobabNodeNetwork
{
	// NodeState
	public enum NodeState
	{
		Follower,
		Candidate,
		Leader
	}

	// LogEntry
	public class LogEntry
	{
		public int Term { get; set; }
		public string Command { get; set; }
	}

	public class Node
	{
		public int Id { get; private set; }
		public NodeState State { get; private set; }
		public int CurrentTerm { get; private set; }
		public int VotedFor { get; private set; }
		public List<LogEntry> Log { get; private set; }
		private NodeChannel channel { get; set; } = new NodeChannel();

		public Node(int id)
		{
			Id = id;
			State = NodeState.Follower;
			CurrentTerm = 0;
			VotedFor = -1;
			Log = new List<LogEntry>();
			channel.Start(id);
			Task.Run(() => ListenForMessages());
		}

		public void BecomeCandidate()
		{
			State = NodeState.Candidate;
			CurrentTerm++;
			VotedFor = Id;
			// 선거 시작
		}

		public void BecomeLeader()
		{
			State = NodeState.Leader;
			// 리더로서의 초기화 작업
		}

		public void BecomeFollower(int term)
		{
			State = NodeState.Follower;
			CurrentTerm = term;
			VotedFor = -1;
		}

		public async Task<bool> SendMessage(Node target, Message message, int fixedDelayMilliseconds = 1000, double lossProbability = 0.1, int maxRetries = 3)
		{
			Random random = new Random();
			int retryCount = 0;

			while (retryCount < maxRetries)
			{
				if (random.NextDouble() < lossProbability)
				{
					Console.WriteLine($"Message from Node {Id} to Node {target.Id} lost due to simulated packet loss.");
					retryCount++;
					await Task.Delay(fixedDelayMilliseconds); // 재시도 전에 고정 지연
					continue;
				}

				try
				{
					byte[] data = Encoding.UTF8.GetBytes($"{message.Term},{message.SenderId},{message.Command}");
					await channel.SendMessage(data);
					Console.WriteLine($"Message sent from Node {Id} to Node {target.Id}: {message.Command}");

					// 확인 응답 대기
					byte[] ackBuffer = new byte[1024];
					var readTask = channel.ReadMessage(ackBuffer);
					if (readTask.Result > 0)
					{
						var ackBytesRead = readTask.Result;
						string ackMessage = Encoding.UTF8.GetString(ackBuffer, 0, ackBytesRead);
						if (ackMessage == "ACK")
						{
							Console.WriteLine($"ACK received from Node {target.Id}");
							return true; // 성공적으로 전송됨
						}
					}
					else
					{
						Console.WriteLine($"No ACK received from Node {target.Id} within timeout period.");
					}
				}
				catch (SocketException ex)
				{
					Console.WriteLine($"SocketException: {ex.Message}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Unexpected error: {ex.Message}");
				}

				retryCount++;
				if (retryCount < maxRetries)
				{
					await Task.Delay(fixedDelayMilliseconds); // 재시도 전에 고정 지연
				}
			}

			Console.WriteLine($"Failed to send message from Node {Id} to Node {target.Id} after {maxRetries} attempts.");
			return false; // 최대 재시도 횟수 초과
		}

		private async Task ListenForMessages(double lossProbability = 0.1)
		{
			Random random = new Random();
			while (true)
			{
				try
				{
					await channel.Accept();
					byte[] buffer = new byte[1024];
					int bytesRead = await channel.ReadMessage(buffer);
					if (bytesRead > 0)
					{
						if (random.NextDouble() < lossProbability)
						{
							Console.WriteLine($"Message lost due to simulated packet loss.");
							continue; // 패킷 손실 시 메시지 처리를 무시
						}

						string messageData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
						string[] parts = messageData.Split(',');
						Message message = new Message
						{
							Term = int.Parse(parts[0]),
							SenderId = int.Parse(parts[1]),
							Command = parts[2]
						};

						// 인위적인 지연 추가
						await Task.Delay(500); // 500ms 지연

						ReceiveMessage(message);

						// 확인 응답 전송
						byte[] ackData = Encoding.UTF8.GetBytes("ACK");
						await channel.SendMessage(ackData);
						Console.WriteLine($"ACK sent to Node {message.SenderId}");
					}
				}
				catch (SocketException ex)
				{
					Console.WriteLine($"SocketException while receiving message: {ex.Message}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Unexpected error while receiving message: {ex.Message}");
				}
			}
		}

		public void ReceiveMessage(Message message)
		{
			// 메시지 처리 로직
			Console.WriteLine($"Node {Id} received message from Node {message.SenderId}: {message.Command}");
		}

		// 로그 복제, 상태 머신 적용 등의 메서드 추가
	}
}