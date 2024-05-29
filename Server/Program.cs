namespace Project
{
	using BaobobCore;
	using Server;
	using System.Net;

	public class Program
	{
		private static void Main()
		{
			using (GameServer builder = new GameServer(IPAddress.Any, 8888))
			{
				Logger.Trace("Start Server");
				builder.StartListener(CancellationToken.None);

				while (true)
				{
					Thread.Sleep(100);
				}
			}
		}
	}
}