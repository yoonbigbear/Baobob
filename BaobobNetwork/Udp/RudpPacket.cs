namespace BaobabNetwork
{
	using System;

	public class RudpPacket
	{
		public int SequenceNumber { get; set; }
		public byte[]? Data { get; set; }
		public int Checksum { get; set; }

		public byte[] ToBytes()
		{
			byte[] seqBytes = BitConverter.GetBytes(SequenceNumber);
			byte[] checksumBytes = BitConverter.GetBytes(CalculateChecksum(Data!));
			byte[] packet = new byte[seqBytes.Length + checksumBytes.Length + Data!.Length];
			Buffer.BlockCopy(seqBytes, 0, packet, 0, seqBytes.Length);
			Buffer.BlockCopy(checksumBytes, 0, packet, seqBytes.Length, checksumBytes.Length);
			Buffer.BlockCopy(Data, 0, packet, seqBytes.Length + checksumBytes.Length, Data!.Length);
			return packet;
		}

		public static RudpPacket FromBytes(byte[] bytes)
		{
			int seq = BitConverter.ToInt32(bytes, 0);
			int checksum = BitConverter.ToInt32(bytes, 4);
			byte[] data = new byte[bytes.Length - 8];
			Buffer.BlockCopy(bytes, 8, data, 0, data.Length);

			int calculatedChecksum = CalculateChecksum(data);
			if (checksum != calculatedChecksum)
			{
				throw new BaobabInvalidChecksum("체크섬이 틀림");
			}

			return new RudpPacket { SequenceNumber = seq, Checksum = checksum, Data = data };
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