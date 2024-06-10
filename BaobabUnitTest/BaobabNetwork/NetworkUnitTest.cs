using System.Text;

namespace BaobabNetwork
{
	[TestClass]
	public class NetworkUnitTest
	{
		[TestMethod]
		public void RSAXorPublicKey()
		{
			// 서버 생성 및 공개 키 출력
			ServerRSA server = new();
			string publickey = server.PublicKey;

			// 클라이언트 생성 및 대칭 키 생성
			ClientRSA client = new ClientRSA(publickey);
			byte[] symmetricKey = client.GenerateSymmetricKey();

			// 대칭 키 암호화 및 서버로 전송 (여기서는 로컬 시뮬레이션)
			byte[] encryptedSymmetricKey = client.EncryptData(symmetricKey);
			byte[] decryptedSymmetricKey = server.DecryptData(encryptedSymmetricKey);

			// 서버와 클라이언트의 대칭 키 확인
			string symmetricString = Convert.ToBase64String(symmetricKey);
			string decryptedSymmetricString = Convert.ToBase64String(decryptedSymmetricKey);
			Assert.AreEqual(decryptedSymmetricString, symmetricString);

			// 데이터를 XOR 방식으로 암호화
			string originalMessage = "Hello, World!";
			byte[] messageBytes = Encoding.UTF8.GetBytes(originalMessage);
			byte[] encryptedMessage = XorBase.XorEncrypt(messageBytes, decryptedSymmetricKey);

			// 암호화된 메시지
			string encryptedText = Convert.ToBase64String(encryptedMessage);

			// 메시지 복호화
			byte[] decryptedMessage = XorBase.XorEncrypt(encryptedMessage, decryptedSymmetricKey);
			string decryptedText = Encoding.UTF8.GetString(decryptedMessage);

			// 복호화된 메시지
			Assert.AreEqual(originalMessage, decryptedText);
		}

		[TestMethod]
		public void RSAAESPublicKey()
		{
			// 서버 생성 및 공개 키 출력
			ServerRSA server = new();
			string publickey = server.PublicKey;

			// 클라이언트 생성 및 대칭 키 생성
			ClientRSA client = new ClientRSA(publickey);
			byte[] symmetricKey = client.GenerateSymmetricKey();

			// 대칭 키 암호화 및 서버로 전송 (여기서는 로컬 시뮬레이션)
			byte[] encryptedSymmetricKey = client.EncryptData(symmetricKey);
			byte[] decryptedSymmetricKey = server.DecryptData(encryptedSymmetricKey);

			// 서버와 클라이언트의 대칭 키 확인
			string symmetricString = Convert.ToBase64String(symmetricKey);
			string decryptedSymmetricString = Convert.ToBase64String(decryptedSymmetricKey);
			Assert.AreEqual(decryptedSymmetricString, symmetricString);

			// 데이터 AES 암호화 및 전송
			string originalMessage = "Hello, World!";
			byte[] messageBytes = Encoding.UTF8.GetBytes(originalMessage);
			byte[] encryptedMessage = AESBase.EncryptWithAES(messageBytes, decryptedSymmetricKey);

			// 암호화된 메시지
			string encryptedText = Convert.ToBase64String(encryptedMessage);

			// 메시지 복호화
			byte[] decryptedMessage = AESBase.DecryptWithAES(encryptedMessage, decryptedSymmetricKey);
			string decryptedText = Encoding.UTF8.GetString(decryptedMessage);

			// 복호화된 메시지
			Assert.AreEqual(originalMessage, decryptedText);
		}
	}
}