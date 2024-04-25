namespace Server
{
	using System.Net.Sockets;

	internal class UserSession
	{
		public Socket? socket { get; protected set; }
		public SocketAsyncEventArgs? recvArg => throw new NotImplementedException();

		public UserSession(Socket? socket)
		{
			this.socket = socket;

			socket?.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
		}

		public async Task Receive()
		{
			try
			{
				while (true)
				{
				}
			}
			catch
			{
			}
		}

		public void Send()
		{
			throw new NotImplementedException();
		}
	}
}