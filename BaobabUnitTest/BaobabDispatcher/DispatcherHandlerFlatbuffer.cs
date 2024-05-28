﻿using Google.FlatBuffers;
using UnitTest;

namespace BaobabDispatcher
{
	public class DispatcherHandlersIFlatbuffer : HandlerDispatcher<IFlatbufferObject>
	{
		public string? Message1 { get; set; }

		public string? Message2 { get; set; }

		[BaobabDispatcher]
		public void HandlePacket(Packet1 message)
		{
			var packet = UnitTest.Packet1.GetRootAsPacket1(message.ByteBuffer);
			this.Message1 = packet.Name;
		}

		[BaobabDispatcher]
		public async Task AsyncHandlePacket(Packet2 message)
		{
			await Task.Yield();
			var packet = UnitTest.Packet2.GetRootAsPacket2(message.ByteBuffer);
			this.Message2 = packet.Name;
		}
	}
}