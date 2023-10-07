using System.IO;
using System.Security.Cryptography;

namespace SecureLibrary.Utilities.Streams
{
    /// <summary>
    /// Stream that provides AES encrytion/decryption and automatic dispose of utility objects
    /// </summary>
    public class AesStream : Stream
    {
        private readonly Aes Aes;
        private readonly ICryptoTransform Transform;
        private readonly Stream Target;
        private readonly CryptoStream CryptoStream;

        public AesStream(Stream target, byte[] key, bool decrypt, CryptoStreamMode mode)
        {
            Aes = Aes.Create();
            Aes.Key = key;
            Target = target;

            if (decrypt)
            {
                byte[] iv = new byte[Aes.BlockSize / 8];
                Target.Read(iv);
                Aes.IV = iv;
            }
            else
            {
                Aes.GenerateIV();
                Target.Write(Aes.IV);
            }

            Transform = decrypt ? Aes.CreateDecryptor() : Aes.CreateEncryptor();
            CryptoStream = new(Target, Transform, mode, true);
        }

        public override bool CanRead => CryptoStream.CanRead;
        public override bool CanSeek => CryptoStream.CanSeek;
        public override bool CanWrite => CryptoStream.CanWrite;
        public override long Length => CryptoStream.Length;
        public override long Position { get => CryptoStream.Position; set => CryptoStream.Position = value; }

        public override void Flush()
        {
            CryptoStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return CryptoStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return CryptoStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            CryptoStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            CryptoStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CryptoStream.Dispose();
                Target.Dispose();
                Transform.Dispose();
                Aes.Dispose();
            }
        }
    }
}
