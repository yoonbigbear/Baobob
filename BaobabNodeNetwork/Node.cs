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
			// ���� ����
		}

		public void BecomeLeader()
		{
			State = NodeState.Leader;
			// �����μ��� �ʱ�ȭ �۾�
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
					await Task.Delay(fixedDelayMilliseconds); // ��õ� ���� ���� ����
					continue;
				}

				try
				{
					byte[] data = Encoding.UTF8.GetBytes($"{message.Term},{message.SenderId},{message.Command}");
					await channel.SendMessage(data);
					Console.WriteLine($"Message sent from Node {Id} to Node {target.Id}: {message.Command}");

					// Ȯ�� ���� ���
					byte[] ackBuffer = new byte[1024];
					var readTask = channel.ReadMessage(ackBuffer);
					if (readTask.Result > 0)
					{
						var ackBytesRead = readTask.Result;
						string ackMessage = Encoding.UTF8.GetString(ackBuffer, 0, ackBytesRead);
						if (ackMessage == "ACK")
						{
							Console.WriteLine($"ACK received from Node {target.Id}");
							return true; // ���������� ���۵�
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
					await Task.Delay(fixedDelayMilliseconds); // ��õ� ���� ���� ����
				}
			}

			Console.WriteLine($"Failed to send message from Node {Id} to Node {target.Id} after {maxRetries} attempts.");
			return false; // �ִ� ��õ� Ƚ�� �ʰ�
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
							continue; // ��Ŷ �ս� �� �޽��� ó���� ����
						}

						string messageData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
						string[] parts = messageData.Split(',');
						Message message = new Message
						{
							Term = int.Parse(parts[0]),
							SenderId = int.Parse(parts[1]),
							Command = parts[2]
						};

						// �������� ���� �߰�
						await Task.Delay(500); // 500ms ����

						ReceiveMessage(message);

						// Ȯ�� ���� ����
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
			// �޽��� ó�� ����
			Console.WriteLine($"Node {Id} received message from Node {message.SenderId}: {message.Command}");
		}

		// �α� ����, ���� �ӽ� ���� ���� �޼��� �߰�
	}
}