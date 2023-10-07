using SecureLibrary.Utilities;
using SecureLibrary.Utilities.Streams;
using System.Drawing;
using System.Drawing.Imaging;

namespace SecureLibrary.Tests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void LsbTest()
        {
            for (int i = 0; i < 1024; i++)
            {
                int len = 1024 * 11 + i;
                ArraySpan<byte> bytes = new(len);
                ArraySpan<byte> bytesRead = new(len);
                ArraySpan<byte> container = new(len + len / 3);
                Random.Shared.NextBytes(container.Source);
                Random.Shared.NextBytes(bytes.Source);

                using (LsbStream writer = new(container))
                {
                    writer.Write(bytes.Source);
                }

                using (LsbStream reader = new(container))
                {
                    reader.Read(bytesRead.Source);
                }

                for (int j = 0; j < len; j++)
                {
                    Assert.AreEqual(bytes[j], bytesRead[j]);
                }
            }
        }

        [TestMethod]
        public void ImageLockerTest()
        {
            byte[] data = new byte[16 * 16 * 3];
            byte[] dataRead = new byte[data.Length];
            Random.Shared.NextBytes(data);
            using (Bitmap bitmap = new(16, 16))
            {
                using (ImageLocker locker = new(bitmap, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb, true))
                {
                    ((ISpan<byte>)new ArraySpan<byte>(data)).CopyTo(locker);
                }
                bitmap.Save("temp.png");
            }
            using Bitmap bitmapLoaded = new("temp.png");
            using (ImageLocker locker = new(bitmapLoaded, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb, true))
            {
                ((ISpan<byte>)locker).CopyTo(new ArraySpan<byte>(dataRead));
            }

            for (int i = 0; i < data.Length; i++)
            {
                Assert.AreEqual(data[i], dataRead[i]);
            }
        }
    }
}