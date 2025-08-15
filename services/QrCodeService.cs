// Path: LedgerLink/Services/QrCodeService.cs
using QRCoder;
using System;
using System.IO;
using System.Drawing; // <--- ADD THIS USING STATEMENT! Required for System.Drawing.Color
// You might also need to install the System.Drawing.Common NuGet package if you haven't already.

namespace LedgerLink.Services
{
    public class QrCodeService
    {
        /// <summary>
        /// Generates a QR code image as a byte array from a given string.
        /// This method is used to create the scannable QR code for a customer's Barcode ID.
        /// </summary>
        /// <param name="dataToEncode">The string data to encode in the QR code (e.g., customer.Barcode GUID string).</param>
        /// <param name="pixelsPerModule">Optional: The size of each square 'module' in the QR code in pixels.
        ///                                  Larger values result in a larger, more detailed image.</param>
        /// <param name="darkColorHex">Optional: Hex color for the dark modules (e.g., "#000000" for black).</param>
        /// <param name="lightColorHex">Optional: Hex color for the light modules (e.g., "#FFFFFF" for white).</param>
        /// <returns>A byte array representing the QR code image in PNG format.</returns>
        /// 
        /// 
        public byte[] GenerateQrCode(string dataToEncode, int pixelsPerModule = 20,
                                     string darkColorHex = "#000000", string lightColorHex = "#FFFFFF")
        {
            // Input validation
            if (string.IsNullOrEmpty(dataToEncode))
            {
                throw new ArgumentException("Data to encode cannot be null or empty.", nameof(dataToEncode));
            }
            if (pixelsPerModule <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pixelsPerModule), "Pixels per module must be greater than 0.");
            }

            // 1. Create a QR code generator instance.
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            // 2. Create the QR code data from the input string.
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(dataToEncode, QRCodeGenerator.ECCLevel.H);

            // 3. Create a PNG renderer.
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);

            // 4. Get the QR code image as a byte array.
            // CRITICAL FIX: Convert hex strings to System.Drawing.Color objects for BOTH parameters
            byte[] qrCodeBytes = qrCode.GetGraphic(
                pixelsPerModule,
                ColorTranslator.FromHtml(darkColorHex),  // <--- CONVERSION HERE for dark color
                ColorTranslator.FromHtml(lightColorHex)  // <--- CONVERSION HERE for light color
            );

            return qrCodeBytes;
        }
    }
}