namespace BaobabUnitTest
{
	using System.Reflection;

	[TestClass]
	public class DispatcherUnitTest
	{
		[TestMethod]
		public void RegisterBaobobDispatchAttribute()
		{
			DispatcherHandlers dispatcher = new DispatcherHandlers();
			dispatcher.BindHandlers(Assembly.GetExecutingAssembly());

			Assert.IsTrue(dispatcher.Count == 2);
		}

		[TestMethod]
		public void HandlerMessageEqual()
		{
			DispatcherHandlers dispatcher = new DispatcherHandlers();
			dispatcher.BindHandlers(Assembly.GetExecutingAssembly());

			string message1 = "From Message1";
			dispatcher?.Invoke(new Message1 { Message = message1 });
			Assert.AreEqual(dispatcher?.Message1, message1);

			string message2 = "From Message2";
			dispatcher?.Invoke(new Message2 { Message = message2 });
			Assert.AreEqual(dispatcher?.Message2, message2);
		}

		[TestMethod]
		public void HandlerAsyncCaller()
		{
			DispatcherHandlers dispatcher = new DispatcherHandlers();
			dispatcher.BindHandlers(Assembly.GetExecutingAssembly());

			Assert.IsFalse(dispatcher?.IsAsyncCall(new Message1()));
			Assert.IsTrue(dispatcher?.IsAsyncCall(new Message2()));
		}
	}
}