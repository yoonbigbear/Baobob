using System;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace BaobabNetwork.TlsHandShake
{
	internal class TlsHandShake
	{
		private static X509Certificate2 serverCertificate = null;

		private void CertificateServer(string certificatePath, string certificatePassword)
		{
			serverCertificate = new X509Certificate2(certificatePath, certificatePassword);
		}

		public void SslAuthenticateAsServer(TcpClient client)
		{
			SslStream sslStream = new SslStream(client.GetStream(), false);
			try
			{
				sslStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls12, true);
				Console.WriteLine("Client connected with SSL.");
			}
			catch (AuthenticationException e)
			{
			}
			finally
			{
				sslStream.Close();
			}
		}

		public void CertifrcateClient(TcpClient client)
		{
		}

		public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
				return true;

			Console.WriteLine($"Certificate error: {sslPolicyErrors}");
			return false;
		}
	}
}