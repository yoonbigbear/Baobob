namespace BaobabRPC
{
	using System;
	using System.IO;
	using System.Net;
	using System.Net.Sockets;
	using System.Text.Json;
	using System.Threading.Tasks;

	public class RpcServer
	{
		private TcpListener _listener;

		public RpcServer(string ipAddress, int port)
		{
			_listener = new TcpListener(IPAddress.Parse(ipAddress), port);
		}

		public void Start()
		{
			_listener.Start();
			Console.WriteLine("RPC Server started...");
			Task.Run(() => ListenForClients());
		}

		private async Task ListenForClients()
		{
			while (true)
			{
				var client = await _listener.AcceptTcpClientAsync();
				Task.Run(() => HandleClient(client));
			}
		}

		private async Task HandleClient(TcpClient client)
		{
			using (var stream = client.GetStream())
			{
				using (var reader = new StreamReader(stream))
				using (var writer = new StreamWriter(stream))
				{
					while (true)
					{
						try
						{
							var requestJson = await reader.ReadLineAsync();
							var request = JsonSerializer.Deserialize<RpcRequest>(requestJson);
							var response = ProcessRequest(request);
							var responseJson = JsonSerializer.Serialize(response);
							await writer.WriteLineAsync(responseJson);
							await writer.FlushAsync();
						}
						catch (Exception ex)
						{
							var errorResponse = new RpcResponse { Error = ex.Message };
							var errorResponseJson = JsonSerializer.Serialize(errorResponse);
							await writer.WriteLineAsync(errorResponseJson);
							await writer.FlushAsync();
						}
					}
				}
			}
		}

		private RpcResponse ProcessRequest(RpcRequest request)
		{
			var response = new RpcResponse();
			try
			{
				if (request.Method == "Add" && request.Parameters.Length == 2)
				{
					var a = Convert.ToInt32(request.Parameters[0]);
					var b = Convert.ToInt32(request.Parameters[1]);
					response.Result = a + b;
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
	}

	public class Program
	{
		public static void Main(string[] args)
		{
			var server = new RpcServer("127.0.0.1", 5000);
			server.Start();
			Console.ReadLine(); // 서버가 종료되지 않도록 하기 위해
		}
	}
}