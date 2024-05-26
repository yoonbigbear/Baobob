namespace BaobabMessageSerializer
{
	public class BaobabSerializableAttribute : Attribute
	{
	}

	public class MessageSerializer
	{
	}

	[BaobabSerializable]
	public struct Message2Args : IMessage
	{
		public int MessageID => throw new NotImplementedException();

		public int arg1 { get; set; }

		public string arg2 { get; set; }
	}

	[BaobabSerializable]
	public struct Message3Args : IMessage
	{
		public int MessageID => throw new NotImplementedException();

		public int arg1 { get; set; }

		public string arg2 { get; set; }

		public List<float> arg3 { get; set; }
	}

	public enum MessageID
	{
		Unknown,
		End,
	}
}