namespace BaobabRPC
{
	public class ClientStub
	{
		private RpcClient rpcClient;

		public ClientStub(string ipAddress, int port)
		{
			rpcClient = new RpcClient(ipAddress, port);
		}

		public int Add(int a, int b)
		{
			var response = rpcClient.Call("Add", a, b);
			if (response.Error == null)
			{
				return (int)response.Result;
			}
			else
			{
				throw new System.Exception(response.Error);
			}
		}

		public void Close()
		{
			rpcClient.Close();
		}
	}
}