namespace BaobabNetwork.Tcp
{
	using System.IO;
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct TcpPayload
	{
		public int ProtocolId { get; set; }
		public short Length { get; set; }
		public bool Encrypted { get; set; }
		public bool Compressed { get; set; }
		public byte[] Data { get; set; }

		public static byte[] Serialize(int id, byte[] bytes, bool encrypted = false, bool compressed = false)
		{
			var buf = new byte[8 + bytes.Length];
			using (MemoryStream ms = new MemoryStream(buf))
			using (BinaryWriter bw = new BinaryWriter(ms))
			{
				bw.Write(id);
				bw.Write((short)bytes.Length);
				bw.Write(encrypted);
				bw.Write(compressed);
				bw.Write(bytes);
				return buf;
			}
		}

		public static void Deserialize(ref TcpPayload payload, byte[] packet)
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