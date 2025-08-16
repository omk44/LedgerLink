// Path: LedgerLink/Services/QrCodeService.cs
using QRCoder;
using System;
using System.Drawing;

namespace LedgerLink.Services
{
    public class QrCodeService
    {
        /// <summary>
        /// Generates a QR code image as a byte array from a given Guid.
        /// </summary>
        /// <param name="guidToEncode">The Guid to encode in the QR code (e.g., customer.Barcode).</param>
        /// <param name="pixelsPerModule">Optional: Size of each QR code module in pixels.</param>
        /// <param name="darkColorHex">Optional: Hex color for dark modules.</param>
        /// <param name="lightColorHex">Optional: Hex color for light modules.</param>
        /// <returns>A byte array representing the QR code image in PNG format.</returns>
        public byte[] GenerateQrCode(Guid guidToEncode, int pixelsPerModule = 20,
                                     string darkColorHex = "#000000", string lightColorHex = "#FFFFFF")
        {
            // CRITICAL FIX: Convert Guid to string for encoding
            string dataToEncode = guidToEncode.ToString();

            if (string.IsNullOrEmpty(dataToEncode))
            {
                throw new ArgumentException("Data to encode cannot be null or empty.", nameof(guidToEncode));
            }
            if (pixelsPerModule <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pixelsPerModule), "Pixels per module must be greater than 0.");
            }

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(dataToEncode, QRCodeGenerator.ECCLevel.H);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

            byte[] qrCodeBytes = qrCode.GetGraphic(
                pixelsPerModule,
                ColorTranslator.FromHtml(darkColorHex),
                ColorTranslator.FromHtml(lightColorHex)
            );

            return qrCodeBytes;
        }
    }
}