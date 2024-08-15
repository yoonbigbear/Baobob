using System.Threading.Tasks;
using System;

namespace BaobabNodeNetwork
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			// Raft 클러스터 테스트
			RaftCluster cluster = new RaftCluster();

			// 노드 간의 상호작용 시뮬레이션
			Node node0 = cluster.GetNode(0);
			Node node1 = cluster.GetNode(1);
			Node node2 = cluster.GetNode(2);

			// 노드 0이 후보가 되어 선거를 시작
			node0.BecomeCandidate();
			await cluster.StartElection(node0).ConfigureAwait(false);

			// 노드 0이 노드 1과 노드 2에게 메시지 전송 (지연 및 패킷 손실 추가)
			Message message = new Message
			{
				Term = node0.CurrentTerm,
				SenderId = node0.Id,
				Command = "TestMessage"
			};

			bool success1 = await node0.SendMessage(node1, message, 1000, 0.2, 3).ConfigureAwait(false); // 1초 지연, 20% 패킷 손실 확률, 최대 3회 재시도
			bool success2 = await node0.SendMessage(node2, message, 1000, 0.2, 3).ConfigureAwait(false); // 1초 지연, 20% 패킷 손실 확률, 최대 3회 재시도

			if (success1)
			{
				Console.WriteLine("Message successfully sent to Node 1");
			}
			else
			{
				Console.WriteLine("Failed to send message to Node 1");
			}

			if (success2)
			{
				Console.WriteLine("Message successfully sent to Node 2");
			}
			else
			{
				Console.WriteLine("Failed to send message to Node 2");
			}

			// 잠시 대기하여 메시지가 처리될 시간을 줌
			await Task.Delay(3000).ConfigureAwait(false);

			// 테스트 종료
			Console.WriteLine("Test completed.");

			Console.ReadLine();
		}
	}
}