namespace BaobabNetwork
{
	using System;
	using System.Buffers;
	using System.Collections.Generic;
	using System.Net.Sockets;
	using System.Threading;
	using System.Threading.Tasks;

	public partial class TcpSession : IDisposable
	{
		public int SessionId { get; set; }
		private NetworkStream tcpStream { get; }
		private bool disposedValue;
		private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

		public TcpSession(Socket socket, TimeSpan heartbeatInterval, TimeSpan heartbeatTimeout)
		{
			tcpStream = new NetworkStream(socket);
			this.heartbeatInterval = heartbeatInterval;
			this.heartbeatTimeout = heartbeatTimeout;
		}

		protected async Task ReadAsync()
		{
			try
			{
				while (true)
				{
					var buffer = ArrayPool<byte>.Shared.Rent(1024);
					var byteReceived = await tcpStream.ReadAsync(buffer, cancellationTokenSource.Token).ConfigureAwait(false);

					DeserializeMessage(buffer, byteReceived);

					ArrayPool<byte>.Shared.Return(buffer);
				}
			}
			finally
			{
				cancellationTokenSource.Dispose();
				tcpStream.Close();
			}
		}

		protected async Task WriteAsync(ReadOnlyMemory<byte> buffer)
		{
			await tcpStream.WriteAsync(buffer).ConfigureAwait(false);
		}

		protected virtual void DeserializeMessage(byte[] buffer, int byteRecevied)
		{
			throw new NotImplementedException();
		}

		protected virtual void SerializeMessage(ReadOnlyMemory<byte> buffer)
		{
			throw new NotImplementedException();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: 관리형 상태(관리형 개체)를 삭제합니다.
					cancellationTokenSource.Dispose();
					tcpStream.Dispose();
				}

				// TODO: 비관리형 리소스(비관리형 개체)를 해제하고 종료자를 재정의합니다.
				// TODO: 큰 필드를 null로 설정합니다.
				disposedValue = true;
			}
		}

		// // TODO: 비관리형 리소스를 해제하는 코드가 'Dispose(bool disposing)'에 포함된 경우에만 종료자를 재정의합니다.
		// ~TcpSession()
		// {
		//     // 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
		//     Dispose(disposing: false);
		// }

		void IDisposable.Dispose()
		{
			// 이 코드를 변경하지 마세요. 'Dispose(bool disposing)' 메서드에 정리 코드를 입력합니다.
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}