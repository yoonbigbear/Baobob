using BaobabNetwork;

namespace BaobabUnitTest.BaobabRudp
{
	internal class RudpTestServer : RudpServer
	{
		public EventHandler<byte[]> ReceiveEvent { get; set; }

		public RudpTestServer(int port) : base(port)
		{ }

		protected override void ProcessData(byte[] bytes) => ReceiveEvent?.Invoke(this, bytes);
	}
}