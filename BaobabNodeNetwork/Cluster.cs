using System.Collections.Generic;
using System.Threading.Tasks;

namespace BaobabNodeNetwork
{
	public class Cluster
	{
		private List<Node> nodes;

		public Cluster(int nodeCount)
		{
			nodes = new List<Node>();
			for (int i = 0; i < nodeCount; i++)
			{
				nodes.Add(new Node(i));
			}
		}

		public Node? GetNode(int id)
		{
			return nodes.Find(node => node.Id == id);
		}

		public async Task StartElection(Node candidate)
		{
			// ���� ���� ����
			foreach (var node in nodes)
			{
				if (node.Id != candidate.Id)
				{
					var message = new Message
					{
						Term = candidate.CurrentTerm,
						SenderId = candidate.Id,
						Command = "RequestVote"
					};
					await candidate.SendMessage(node, message);
				}
			}
		}

		public async Task SendAppendEntries(Node leader, LogEntry entry)
		{
			// �α� ���� ����
			foreach (var node in nodes)
			{
				if (node.Id != leader.Id)
				{
					var message = new Message
					{
						Term = leader.CurrentTerm,
						SenderId = leader.Id,
						Command = "AppendEntries"
					};
					await leader.SendMessage(node, message);
				}
			}
		}

		// ��Ÿ Ŭ������ ���� �޼��� �߰�
	}
}