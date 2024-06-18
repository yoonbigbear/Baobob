using System;

namespace BaobabRPC
{
	public class ServerStub
	{
		public RpcResponse ProcessRequest(RpcRequest request)
		{
			var response = new RpcResponse();
			try
			{
				if (request.Method == "Add" && request.Parameters.Length == 2)
				{
					var a = Convert.ToInt32(request.Parameters[0]);
					var b = Convert.ToInt32(request.Parameters[1]);
					response.Result = Add(a, b);
				}
				else
				{
					response.Error = "Unknown method or invalid parameters";
				}
			}
			catch (Exception ex)
			{
				response.Error = ex.Message;
			}
			return response;
		}

		private int Add(int a, int b)
		{
			return a + b;
		}
	}
}