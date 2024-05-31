namespace BaobabNetwork
{
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential)]
	public struct Payload
	{
		public int ProtocolId { get; set; }
		public int Length { get; set; }
		public bool Encrypted { get; set; }
		public bool Compressed { get; set; }
		public byte[] Data { get; set; }

		public static byte[] Serialize(int id, byte[] bytes, bool encrypted = false, bool compressed = false)
		{
			var buf = new byte[8 + bytes.Length];
			using (MemoryStream ms = new MemoryStream(buf))
			using (BinaryWriter bw = new BinaryWriter(ms))
			{
				bw.Write((int)id);
				bw.Write((ushort)bytes.Length);
				bw.Write((bool)encrypted);
				bw.Write((bool)compressed);
				bw.Write(bytes);
				return buf;
			}
		}

		public static void Deserialize(ref Payload payload, byte[] packet)
		{
			using (MemoryStream ms = new MemoryStream(packet))
			using (BinaryReader br = new BinaryReader(ms))
			{
				payload.ProtocolId = br.ReadInt32();
				payload.Length = br.ReadInt16();
				payload.Encrypted = br.ReadBoolean();
				payload.Compressed = br.ReadBoolean();
				payload.Data = br.ReadBytes(payload.Length);
			}
		}
	}
}