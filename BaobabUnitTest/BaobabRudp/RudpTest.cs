namespace BaobabUnitTest.BaobabRudp
{
	using BaobabNetwork;

	[TestClass]
	public class RudpTest
	{
		private RudpClient client = new RudpClient("127.0.0.1", 9000);
		private RudpTestServer server = new RudpTestServer(9000);

		[TestInitialize]
		public void Initialize()
		{
			_ = server.StartAsync().ConfigureAwait(false);
		}

		[TestMethod]
		public async Task RudpMessageTest()
		{
			AutoResetEvent autoResetEvent = new AutoResetEvent(false);
			server.ReceiveEvent += (_, _) =>
			{
				autoResetEvent.Set();
			};

			await client.SendAsync($"message").ConfigureAwait(false);

			Assert.IsTrue(autoResetEvent.WaitOne(1000));
		}
	}
}