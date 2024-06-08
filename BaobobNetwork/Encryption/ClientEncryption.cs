namespace BaobabNetwork
{
	using System.Net.Sockets;
	using System.Security.Cryptography;

	internal class ClientEncryption
	{
		private static RSAParameters serverPublicKey;

		/// <summary>
		/// 서버로부터 공개키를 받습니다.
		/// </summary>
		/// <param name="stream"> 연결된 서버 Network stream </param>
		private static void ReceivePublicKey(NetworkStream stream)
		{
			byte[] publicKey = new byte[2048 / 8]; // RSA 2048 비트 사용 시 크기
			stream.Read(publicKey, 0, publicKey.Length);
			using (RSA rsa = RSA.Create())
			{
				rsa.ImportRSAPublicKey(publicKey, out _);
				serverPublicKey = rsa.ExportParameters(false);
			}
		}

		/// <summary>
		/// 클라이언트에서 생성한 Session Key를 암호화하여 서버로 전송합니다.
		/// </summary>
		/// <param name="sessionKey"> 클라이언트에서 생성한 Session key </param>
		/// <returns> 암호화된 세션키 byte배열 </returns>
		private static byte[] EncryptSessionKey(byte[] sessionKey)
		{
			using (RSA rsa = RSA.Create())
			{
				rsa.ImportParameters(serverPublicKey);
				return rsa.Encrypt(sessionKey, RSAEncryptionPadding.OaepSHA256);
			}
		}
	}
}