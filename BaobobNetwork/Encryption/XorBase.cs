namespace BaobabNetwork
{
	public class XorBase
	{
		public static byte[] XorEncrypt(byte[] data, byte[] key)
		{
			byte[] result = new byte[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				result[i] = (byte)(data[i] ^ key[i % key.Length]);
			}
			return result;
		}
	}
}