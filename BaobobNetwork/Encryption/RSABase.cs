namespace BaobabNetwork
{
	using System;
	using System.Security.Cryptography;

	public class RSABase : IDisposable
	{
		private RSACryptoServiceProvider rsa;

		public string PublicKey { get; set; }
		public string PrivateKey { get; set; }

		public RSABase()
		{
			rsa = new RSACryptoServiceProvider(2048);
			rsa.PersistKeyInCsp = false;
			PublicKey = Convert.ToBase64String(rsa.ExportCspBlob(false));
			PrivateKey = Convert.ToBase64String(rsa.ExportCspBlob(true));
		}

		public byte[] DecryptKey(byte[] data)
		{
			rsa.ImportCspBlob(Convert.FromBase64String(PrivateKey));
			return rsa.Decrypt(data, false);
		}

		public byte[] EncryptKey(byte[] symmetricKey)
		{
			rsa.PersistKeyInCsp = false;
			rsa.ImportCspBlob(Convert.FromBase64String(PublicKey));
			return rsa.Encrypt(symmetricKey, false);
		}

		public void Dispose()
		{
			rsa.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	public class AESBase : IDisposable
	{
		private Aes aes { get; set; }
		private ICryptoTransform cryptoTransform { get; set; }

		public AESBase()
		{
			aes = Aes.Create();
			aes.GenerateKey();
			aes.Mode = CipherMode.ECB; // ECB 모드는 예제용으로 사용하며, 실제 구현 시에는 다른 모드를 사용해야 함
			aes.Padding = PaddingMode.PKCS7; // 패딩 모드 설정
			cryptoTransform = aes.CreateDecryptor();
		}

		//Encrypt Data with symmetric key
		public byte[] EncryptData(byte[] data) => cryptoTransform.TransformFinalBlock(data, 0, data.Length);

		// decrypte data with SymmeticKey
		public byte[] DecryptData(byte[] data, byte[] symmetricKey) => cryptoTransform.TransformFinalBlock(data, 0, data.Length);

		public byte[] GenerateSymmetricKey() => aes.Key;

		public void SetKey(byte[] key) => aes.Key = key;

		public void Dispose()
		{
			aes.Dispose();
			cryptoTransform.Dispose();
			GC.SuppressFinalize(this);
		}
	}

	// xor 방식의 키 생성 및 암호화, 복호화 코드
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

		public static byte[] XorDecrypt(byte[] data, byte[] key) => XorBase.XorEncrypt(data, key);

		public static byte[] GenerateKey(int length)
		{
			byte[] key = new byte[length];
			Random random = new Random();
			random.NextBytes(key);
			return key;
		}
	}
}