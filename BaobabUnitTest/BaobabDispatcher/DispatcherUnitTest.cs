namespace BaobabUnitTest
{
	using BaobabDispatcher;
	using Google.FlatBuffers;
	using System.Reflection;
	using UnitTest;

	[TestClass]
	public class DispatcherUnitTest
	{
		[TestMethod]
		public void RegisterBaobobDispatchAttribute()
		{
			{
				DispatcherHandlersIMessage.BindHandlerIMessageType(Assembly.GetExecutingAssembly());

				Assert.IsTrue(DispatcherHandlersIMessage.Count == 2);
			}

			{
				DispatcherHandlersIFlatbuffer.BindHandlerIFlatbufferType(Assembly.GetExecutingAssembly());

				Assert.IsTrue(DispatcherHandlersIFlatbuffer.Count == 2);
			}
		}

		[TestMethod]
		public async Task HandlerMessageEqual()
		{
			{
				DispatcherHandlersIMessage.BindHandlerIMessageType(Assembly.GetExecutingAssembly());

				string message1arg = "From Message1";
				var message1 = new Message1 { Message = message1arg };
				_ = DispatcherHandlersIMessage.Invoke(message1.MessageID, message1);
				Assert.AreEqual(DispatcherHandlersIMessage.Message1, message1arg);

				string message2arg = "From Message2";
				var message2 = new Message2 { Message = message2arg };
				await DispatcherHandlersIMessage.Invoke(message2.MessageID, message2).ConfigureAwait(false);
				Assert.AreEqual(DispatcherHandlersIMessage.Message2, message2arg);
			}

			{
				DispatcherHandlersIFlatbuffer.BindHandler(Assembly.GetExecutingAssembly());
				{
					string message1arg = "From Message1";

					//write
					var builder = new FlatBufferBuilder(128);
					var offset = Packet1.CreatePacket1(builder, builder.CreateSharedString(message1arg));
					builder.Finish(offset.Value);
					var buffer = builder.DataBuffer;

					//read
					var packet1 = Packet1.GetRootAsPacket1(buffer);
					_ = DispatcherHandlersIFlatbuffer.Invoke(typeof(Packet1).FullName!.GetHashCode(), packet1);
					Assert.AreEqual(DispatcherHandlersIFlatbuffer.Message1, message1arg);
				}

				{
					string message2arg = "From Message2";

					//write
					var builder = new FlatBufferBuilder(128);
					var offset = Packet2.CreatePacket2(builder, builder.CreateSharedString(message2arg));
					builder.Finish(offset.Value);
					var buffer = builder.DataBuffer;

					//read
					var packet2 = Packet2.GetRootAsPacket2(buffer);
					await DispatcherHandlersIFlatbuffer.Invoke(typeof(Packet2).FullName!.GetHashCode(), packet2)!.ConfigureAwait(false);
					Assert.AreEqual(DispatcherHandlersIFlatbuffer.Message2, message2arg);
				}
			}
		}

		[TestMethod]
		public void HandlerAsyncCaller()
		{
			{
				DispatcherHandlersIMessage dispatcher = new DispatcherHandlersIMessage();
				DispatcherHandlersIMessage.BindHandlerIMessageType(Assembly.GetExecutingAssembly());

				var message1 = new Message1();
				var message2 = new Message2();
				Assert.IsFalse(DispatcherHandlersIMessage.IsAsyncCall(message1.MessageID, message1));
				Assert.IsTrue(DispatcherHandlersIMessage.IsAsyncCall(message2.MessageID, message2));
			}
		}
	}
}