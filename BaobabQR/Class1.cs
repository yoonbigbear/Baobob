using QRCoder;

namespace BaobabQR
{
	public class QRGenerator
	{
		private static void Generate(string url)
		{
			using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
			using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
			using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
			{
				byte[] qrCodeImage = qrCode.GetGraphic(20);
			}
		}
	}
}