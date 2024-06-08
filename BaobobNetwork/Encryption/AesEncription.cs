namespace BaobabNetwork
{
	using System.IO;
	using System.Security.Cryptography;

	public class AESEncription
	{
		/// <summary>
		/// AES Key를 생성합니다.
		/// </summary>
		/// <returns> 생성된 AES의 byte 배열 </returns>
		public static byte[] GenerateRandomKey()
		{
			using (Aes aes = Aes.Create())
			{
				aes.GenerateKey();
				return aes.Key;
			}
		}

		/// <summary>
		/// AES를 이용해 데이터를 암호화 합니다.
		/// </summary>
		/// <param name="data"> 암호화할 데이터 원본 </param>
		/// <param name="sessionKey"> Session key </param>
		/// <returns> 암호화된 byte 배열을 반환 </returns>
		public static byte[] EncryptData(byte[] data, byte[] sessionKey)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = sessionKey;
				aes.IV = new byte[16]; // 기본 IV, 실제 구현에서는 클라이언트/서버와 공유하는 IV 사용
				using (MemoryStream msEncrypt = new MemoryStream())
				using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
				{
					csEncrypt.Write(data, 0, data.Length);
					csEncrypt.FlushFinalBlock();
					return msEncrypt.ToArray();
				}
			}
		}

		/// <summary>
		/// 수신된 데이터를 AES방식으로 복호화 합니다.
		/// </summary>
		/// <param name="encryptedData"> 암호화된 데이터 </param>
		/// <param name="sessionKey"> Session Key </param>
		/// <returns> 복호화된 데이터 원본 </returns>
		public static byte[] DecryptData(byte[] encryptedData, byte[] sessionKey)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = sessionKey;
				aes.IV = new byte[16]; // 기본 IV, 실제 구현에서는 클라이언트/서버와 공유하는 IV 사용
				using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
				using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, aes.CreateDecryptor(), CryptoStreamMode.Read))
				using (MemoryStream msOutput = new MemoryStream())
				{
					csDecrypt.CopyTo(msOutput);
					return msOutput.ToArray();
				}
			}
		}
	}
}