namespace BaobabNetwork
{
	using System.Collections.Concurrent;
	using System.Net.Sockets;
	using System.Security.Cryptography;

	public class ServerEncryption
	{
		private static RSA? rsa;
		private static ConcurrentDictionary<int, byte[]> sessionKeys = new ConcurrentDictionary<int, byte[]>();

		/// <summary>
		/// RSA 키 쌍을 생성합니다.
		/// </summary>
		public static void CreateRSA()
		{
			rsa = RSA.Create();
			rsa.KeySize = 2048;
		}

		/// <summary>
		/// 클라이언트로 공개 키를 전송합니다.
		/// </summary>
		/// <param name="stream"> Client network stream </param>
		public static async void SendNewPublicKey(NetworkStream stream)
		{
			byte[] publicKey = rsa!.ExportRSAPublicKey();
			await stream.WriteAsync(publicKey, 0, publicKey.Length);
		}

		/// <summary>
		/// SessionKey를 사용하여 데이터를 복호화 합니다.
		/// </summary>
		/// <param name="encryptedSessionKey"> 클라이언트로부터 SessionKey를 이용하여 암호화된 데이터 </param>
		/// <returns></returns>
		private byte[] DecryptSessionKey(byte[] encryptedSessionKey)
		{
			return rsa!.Decrypt(encryptedSessionKey, RSAEncryptionPadding.OaepSHA256);
		}
	}
}