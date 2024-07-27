namespace BaobabNetwork
{
	using BaobabNetwork.Udp;
	using System;

	public class RudpPacket
	{
		internal RudpHeader Header { get; set; }
		public byte[]? Data { get; set; }

		public byte[] Serialize()
		{
			byte[] seqBytes = BitConverter.GetBytes(Header.SequenceNumber);
			byte[] checksumBytes = BitConverter.GetBytes(Header.Checksum);
			var buf = new byte[seqBytes.Length + checksumBytes.Length + Data!.Length];
			Buffer.BlockCopy(seqBytes, 0, buf, 0, seqBytes.Length);
			Buffer.BlockCopy(checksumBytes, 0, buf, seqBytes.Length, checksumBytes.Length);
			Buffer.BlockCopy(Data, 0, buf, seqBytes.Length + checksumBytes.Length, Data!.Length);
			return buf;
		}

		public static RudpPacket Deserialize(byte[] bytes)
		{
			var header = new RudpHeader();
			header.SequenceNumber = BitConverter.ToInt32(bytes, 0);
			header.Checksum = BitConverter.ToInt32(bytes, 4);
			byte[] data = new byte[bytes.Length - 8];
			Buffer.BlockCopy(bytes, 8, data, 0, data.Length);

			int calculatedChecksum = CalculateChecksum(data);
			if (header.Checksum != calculatedChecksum)
			{
				throw new BaobabInvalidRudpHeader("Checksum not matches");
			}

			return new RudpPacket { Header = header, Data = data };
		}

		public static int CalculateChecksum(byte[] data)
		{
			// 간단한 체크섬 계산 로직
			int checksum = 0;
			foreach (byte b in data)
			{
				checksum += b;
			}
			return checksum;
		}
	}
}