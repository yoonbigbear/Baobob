namespace BaobabUnitTest
{
	using BaobabDispatcher;
	using Google.FlatBuffers;
	using System.Reflection;

	[TestClass]
	public class DispatcherUnitTest
	{
		[TestMethod]
		public void RegisterBaobobDispatchAttribute()
		{
			{
				DispatcherHandlersIMessage<IMessage> dispatcher = new DispatcherHandlersIMessage<IMessage>();
				dispatcher.BindHandlerIMessageType(Assembly.GetExecutingAssembly());

				Assert.IsTrue(dispatcher.Count == 2);
			}

			{
				DispatcherHandlersIMessage<IFlatbufferObject> dispatcher = new DispatcherHandlersIMessage<IFlatbufferObject>();
				dispatcher.BindHandlerIMessageType(Assembly.GetExecutingAssembly());
				Assert.IsTrue(dispatcher.Count == 2);
			}
		}

		[TestMethod]
		public void HandlerMessageEqual()
		{
			DispatcherHandlersIMessage<IMessage> dispatcher = new DispatcherHandlersIMessage<IMessage>();
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

		[TestMethod]
		public void HandlerAsyncCaller()
		{
			DispatcherHandlersIMessage<IMessage> dispatcher = new DispatcherHandlersIMessage<IMessage>();
			dispatcher.BindHandlerIMessageType(Assembly.GetExecutingAssembly());

			var message1 = new Message1();
			var message2 = new Message2();
			Assert.IsFalse(dispatcher?.IsAsyncCall(message1.MessageID, message1));
			Assert.IsTrue(dispatcher?.IsAsyncCall(message2.MessageID, message2));
		}
	}
}