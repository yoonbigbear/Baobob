namespace BaobabNetwork
{
	using System;
	using System.Net;
	using System.Net.Http;
	using System.Net.Sockets;
	using System.Threading.Tasks;

	public class ClientBuilder : IDisposable
	{
		protected TcpClient? tcpClient { get; }
		private bool disposedValue;

		public bool Connected { get; set; }

		public ClientBuilder()
		{
			tcpClient = new TcpClient();
		}

		public async Task ConnectAsync(IPAddress ip, short port)
		{
			await tcpClient!.ConnectAsync(ip, port).ConfigureAwait(false);
			AcceptSession(tcpClient.Client);
		}

		public virtual void AcceptSession(Socket? socket)
		{
			Connected = true;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: 관리형 상태(관리형 개체)를 삭제합니다.
					tcpClient?.Dispose();
				}

				// TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
				// TODO: 큰 필드를 null로 설정합니다.
				disposedValue = true;
			}
		}

		// // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
		// ~ClientBuilder()
		// {
		//     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}