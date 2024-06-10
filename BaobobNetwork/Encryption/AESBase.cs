namespace BaobabNetwork
{
	using System.Security.Cryptography;

	public class AESBase
	{
		public static byte[] EncryptWithAES(byte[] data, byte[] key)
		{
			using (var aes = Aes.Create())
			{
				aes.Key = key;
				aes.Mode = CipherMode.ECB; // ECB 모드는 예제용으로 사용하며, 실제 구현 시에는 다른 모드를 사용해야 함
				aes.Padding = PaddingMode.PKCS7; // 패딩 모드 설정
				using (var encryptor = aes.CreateEncryptor())
				{
					return encryptor.TransformFinalBlock(data, 0, data.Length);
				}
			}
		}

		public static byte[] DecryptWithAES(byte[] data, byte[] key)
		{
			using (var aes = Aes.Create())
			{
				aes.Key = key;
				aes.Mode = CipherMode.ECB; // ECB 모드는 예제용으로 사용하며, 실제 구현 시에는 다른 모드를 사용해야 함
				aes.Padding = PaddingMode.PKCS7; // 패딩 모드 설정
				using (var decryptor = aes.CreateDecryptor())
				{
					return decryptor.TransformFinalBlock(data, 0, data.Length);
				}
			}
		}
	}
}