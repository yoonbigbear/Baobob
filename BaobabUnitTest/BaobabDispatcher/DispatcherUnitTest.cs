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

		public void HandlerMessageTest()
		{
			DispatcherHandlers dispatcher = new DispatcherHandlers();
			dispatcher.BindHandlers(Assembly.GetExecutingAssembly());

			string message1 = "From Message1";
			dispatcher?.Invoke(new Message1());
			Assert.AreEqual(dispatcher?.Message1, message1);

			string message2 = "From Message2";
			dispatcher?.Invoke(new Message2());
			Assert.AreEqual(dispatcher?.Message2, message2);
		}
	}
}