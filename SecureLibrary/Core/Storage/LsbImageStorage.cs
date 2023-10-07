using SecureLibrary.Utilities;
using SecureLibrary.Utilities.Streams;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureLibrary.Core.Storage
{
    /// <summary>
    /// A storage that provides a method for storing data in a steganographic LSB image
    /// </summary>
    public class LsbImageStorage : BinaryStorage
    {
        public LsbImageStorage(string fileName)
        {
            FileName = fileName;
        }

        public override string Name => "LSB Image - " + Path.GetFileName(FileName);

        public string FileName { get; }

        public override Stream OpenWrite()
        {
            using FileStream stream = File.OpenRead(FileName);
            return new LsbStream(
                new ImageLocker(new Bitmap(stream), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb,
                                false, FileName)
            );
        }

        public override Stream OpenRead()
        {
            using FileStream stream = File.OpenRead(FileName);
            return new LsbStream(
                new ImageLocker(new Bitmap(stream), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb)
            );
        }
    }
}
