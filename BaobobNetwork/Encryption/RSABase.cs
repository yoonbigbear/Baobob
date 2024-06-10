namespace BaobabNetwork
{
	using System.Security.Cryptography;
	using System;
	using System.Collections.Generic;

	public class RSAKeyPair
	{
		public string PublicKey { get; private set; }
		public string PrivateKey { get; private set; }

		public RSAKeyPair()
		{
			using (var rsa = new RSACryptoServiceProvider(2048))
			{
				rsa.PersistKeyInCsp = false;
				PublicKey = Convert.ToBase64String(rsa.ExportCspBlob(false));
				PrivateKey = Convert.ToBase64String(rsa.ExportCspBlob(true));
			}
		}
	}

	public class ServerRSA
	{
		private RSACryptoServiceProvider rsa;
		public string PublicKey { get; private set; }
		private string PrivateKey { get; set; }

		public ServerRSA()
		{
			rsa = new RSACryptoServiceProvider(2048);
			rsa.PersistKeyInCsp = false;
			PublicKey = Convert.ToBase64String(rsa.ExportCspBlob(false));
			PrivateKey = Convert.ToBase64String(rsa.ExportCspBlob(true));
		}

		public byte[] DecryptData(byte[] data)
		{
			rsa.ImportCspBlob(Convert.FromBase64String(PrivateKey));
			return rsa.Decrypt(data, false);
		}
	}

	public class ClientRSA
	{
		private string serverPublicKey;

		public ClientRSA(string serverPublicKey)
		{
			this.serverPublicKey = serverPublicKey;
		}

		public byte[] EncryptData(byte[] data)
		{
			using (var rsa = new RSACryptoServiceProvider())
			{
				rsa.PersistKeyInCsp = false;
				rsa.ImportCspBlob(Convert.FromBase64String(serverPublicKey));
				return rsa.Encrypt(data, false);
			}
		}

		public byte[] GenerateSymmetricKey()
		{
			using (var aes = Aes.Create())
			{
				aes.GenerateKey();
				return aes.Key;
			}
		}
	}
}