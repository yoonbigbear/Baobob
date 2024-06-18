namespace BaobabRPC
{
	using System.IO;
	using System.Net.Sockets;
	using System.Text.Json;

	public class RpcClient
	{
		private TcpClient _client;
		private NetworkStream _stream;
		private StreamReader _reader;
		private StreamWriter _writer;

		public RpcClient(string ipAddress, int port)
		{
			_client = new TcpClient(ipAddress, port);
			_stream = _client.GetStream();
			_reader = new StreamReader(_stream);
			_writer = new StreamWriter(_stream);
		}

		public RpcResponse Call(string method, params object[] parameters)
		{
			var request = new RpcRequest
			{
				Method = method,
				Parameters = parameters
			};

			var requestJson = JsonSerializer.Serialize(request);
			_writer.WriteLine(requestJson);
			_writer.Flush();

			var responseJson = _reader.ReadLine();
			return JsonSerializer.Deserialize<RpcResponse>(responseJson);
		}

		public void Close()
		{
			_reader.Close();
			_writer.Close();
			_stream.Close();
			_client.Close();
		}
	}
}