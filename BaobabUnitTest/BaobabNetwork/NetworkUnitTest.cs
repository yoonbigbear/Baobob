using System.Text;

namespace BaobabUnitTest
{
	using BaobabNetwork;

	public class ClientRSAAes : RSABase, IDisposable
	{
		public AESBase aes = new();
	}

	public class ServerRSAAes : RSABase, IDisposable
	{
		public AESBase aes = new();
	}

	public class ClientRSAXor : RSABase, IDisposable
	{
		public byte[] key { get; set; }
	}

	public class ServerRSAXor : RSABase, IDisposable
	{
		public byte[] key { get; set; }
	}

	[TestClass]
	public class NetworkUnitTest
	{
		[TestMethod]
		public void RSAXorPublicKey()
		{
		}

		[TestMethod]
		public void RSAAesPublicKey()
		{
			// 클라이언트 공개 키 생성
			ClientRSAAes client = new();
			string publickey = client.PublicKey;

			// 서버로 key 전송

			// 서버 생성 및 대칭 키 생성
			ServerRSAAes server = new ServerRSAAes();
			server.PublicKey = publickey;
			byte[] symmetricKey = server.aes.GenerateSymmetricKey();

			// 대칭 키 암호화 및 클라이언트로 전송 (여기서는 로컬 시뮬레이션)
			byte[] encryptedSymmetricKey = server.EncryptKey(symmetricKey);

			//클라이언트에게 전송

			// 서버로부터 받은 대칭키를 복호화 합니다.
			byte[] decryptedSymmetricKey = client.DecryptKey(encryptedSymmetricKey);

			// 서버와 클라이언트의 대칭 키 확인
			string symmetricString = Convert.ToBase64String(symmetricKey);
			string decryptedSymmetricString = Convert.ToBase64String(decryptedSymmetricKey);
			Assert.AreEqual(decryptedSymmetricString, symmetricString);
			client.aes.SetKey(decryptedSymmetricKey);

			// 데이터 AES 암호화 및 전송
			string originalMessage = "Hello, World!";
			byte[] messageBytes = Encoding.UTF8.GetBytes(originalMessage);
			byte[] encryptedMessage = client.aes.EncryptData(messageBytes);

			// 암호화된 메시지
			string encryptedText = Convert.ToBase64String(encryptedMessage);

			// 메시지 복호화
			byte[] decryptedMessage = server.aes.DecryptData(encryptedMessage, decryptedSymmetricKey);
			string decryptedText = Encoding.UTF8.GetString(decryptedMessage);

			// 복호화된 메시지
			Assert.AreEqual(originalMessage, decryptedText);
		}
	}
}