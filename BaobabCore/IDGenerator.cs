namespace BaobabCore
{
	public class IdGen
	{
		public enum IdType : ulong
		{
			None = 0,
			Object,
			End,
		}

		private static int guidSeqOffset = 0;
		private const ulong typeOffset = 1000;
		private const ulong uniqueOffset = 1000;
		private const ulong timeOffset = 10000000000000;

		public static ulong GenerateGUID(IdType type, int serverid)
		{
			ulong guid = 0;
			var uniqueSeq = (ulong)Interlocked.Increment(ref guidSeqOffset);
			if (uniqueSeq > 900)
				guidSeqOffset = 0;

			//type
			guid += (ulong)type * typeOffset * typeOffset * timeOffset;
			//uniqueSeq
			guid += (ulong)uniqueSeq * uniqueOffset * timeOffset;
			//serverId
			guid += (ulong)serverid * timeOffset;
			//mstime
			guid += (ulong)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());

			return guid;
		}

		private static uint eidSeqOffset = 0;

		public static uint GenerateEID()
		{
			return Interlocked.Increment(ref eidSeqOffset);
		}
	}
}