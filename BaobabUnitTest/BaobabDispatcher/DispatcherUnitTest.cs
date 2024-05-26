namespace BaobabUnitTest
{
	using BaobabDispatcher;
	using System.Reflection;

	[TestClass]
	public class DispatcherUnitTest
	{
		[TestMethod]
		public void RegisterBaobobDispatchAttribute()
		{
			{
				DispatcherHandlersIMessage dispatcher = new DispatcherHandlersIMessage();
				dispatcher.BindHandlerIMessageType(Assembly.GetExecutingAssembly());

				Assert.IsTrue(dispatcher.Count == 2);
			}

			{
				DispatcherHandlersIFlatbuffer dispatcher = new DispatcherHandlersIFlatbuffer();
				dispatcher.BindHandlerIFlatbufferType(Assembly.GetExecutingAssembly());

				Assert.IsTrue(dispatcher.Count == 2);
			}
		}

		[TestMethod]
		public void HandlerMessageEqual()
		{
			{
				DispatcherHandlersIMessage dispatcher = new DispatcherHandlersIMessage();
				dispatcher.BindHandlerIMessageType(Assembly.GetExecutingAssembly());

				string message1arg = "From Message1";
				var message1 = new Message1 { Message = message1arg };
				dispatcher?.Invoke(message1.MessageID, message1);
				Assert.AreEqual(dispatcher?.Message1, message1arg);

				string message2arg = "From Message2";
				var message2 = new Message2 { Message = message2arg };
				dispatcher?.Invoke(message2.MessageID, message2);
				Assert.AreEqual(dispatcher?.Message2, message2arg);
			}

			//{
			//	DispatcherHandlersIFlatbuffer dispatcher = new DispatcherHandlersIFlatbuffer();
			//	dispatcher.BindHandlerIMessageType(Assembly.GetExecutingAssembly());

			//	string message1arg = "From Message1";
			//	var builder = new FlatBufferBuilder(1024);
			//	var name = builder.CreateString(message1arg);
			//	var packet = Packet.CreatePacket(builder, name);

			//	Packet.GetRootAsPacket(packet)
			//	dispatcher?.Invoke(typeof(Packet).FullName.GetHashCode(), );
			//	Assert.AreEqual(dispatcher?.Message1, message1arg);

			//	string message2arg = "From Message2";
			//	var message2 = new Message2 { Message = message2arg };
			//	dispatcher?.Invoke(message2.MessageID, message2);
			//	Assert.AreEqual(dispatcher?.Message2, message2arg);
			//}
		}

		[TestMethod]
		public void HandlerAsyncCaller()
		{
			DispatcherHandlersIMessage dispatcher = new DispatcherHandlersIMessage();
			dispatcher.BindHandlerIMessageType(Assembly.GetExecutingAssembly());

			var message1 = new Message1();
			var message2 = new Message2();
			Assert.IsFalse(dispatcher?.IsAsyncCall(message1.MessageID, message1));
			Assert.IsTrue(dispatcher?.IsAsyncCall(message2.MessageID, message2));
		}
	}
}