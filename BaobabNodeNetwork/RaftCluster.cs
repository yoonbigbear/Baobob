using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace BaobabNodeNetwork
{
	public class RaftCluster
	{
		private List<(int id, Node node)> nodes;

		private List<(int, IPEndPoint)> nodelist = new List<(int, IPEndPoint)>
		{
			(0, new IPEndPoint(IPAddress.Parse("127.0.0.1"),5000)),
			(1, new IPEndPoint(IPAddress.Parse("127.0.0.1"),5001)),
			(2, new IPEndPoint(IPAddress.Parse("127.0.0.1"),5002))
		};

		public RaftCluster()
		{
			nodes = new List<(int, Node)>();

			for (int i = 0; i < nodelist.Count; i++)
			{
				var node = new Node(i, nodelist);
				nodes.Add((i, node));
			}
		}

		public Node? GetNode(int id)
		{
			return nodes.Find(node => node.id == id).Item2;
		}

		public async Task StartElection(Node candidate)
		{
			// 선거 시작 로직
			foreach (var node in nodes)
			{
				if (node.id != candidate.Id)
				{
					var message = new Message
					{
						Term = candidate.CurrentTerm,
						SenderId = candidate.Id,
						Command = "RequestVote"
					};
					await candidate.SendMessage(node.node, message).ConfigureAwait(false);
				}
			}
		}

		public async Task SendAppendEntries(Node leader, LogEntry entry)
		{
			// 로그 복제 로직
			foreach (var node in nodes)
			{
				if (node.id != leader.Id)
				{
					var message = new Message
					{
						Term = leader.CurrentTerm,
						SenderId = leader.Id,
						Command = "AppendEntries"
					};
					await leader.SendMessage(node.node, message).ConfigureAwait(false);
				}
			}
		}

		// 기타 클러스터 관리 메서드 추가
	}
}