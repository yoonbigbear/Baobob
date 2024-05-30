namespace Project
{
	using BaobobCore;
	using Server;
	using System.Net;
	using System.Reflection;

	public class Program
	{
		private static void Main()
		{
			MessageHandler.BindHandler(Assembly.GetExecutingAssembly());

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