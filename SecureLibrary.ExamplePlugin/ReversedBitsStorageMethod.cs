using SecureLibrary.Core.Storage;
using SecureLibrary.Core.Storage.Methods;

namespace SecureLibrary.ExamplePlugin
{
    public class ReversedBitsStorageMethod : StorageMethod
    {
        public static ReversedBitsStorageMethod Instance { get; }

        static ReversedBitsStorageMethod()
        {
            Instance = new();
        }

        protected ReversedBitsStorageMethod() { }

        public override string Name => "Reversed bits";

        public override Stream OpenRead(BinaryStorage storage)
        {
            return new ReversedStream(storage.OpenRead());
        }

        public override Stream OpenWrite(BinaryStorage storage)
        {
            return new ReversedStream(storage.OpenWrite());
        }

        public class ReversedStream : Stream
        {
            public ReversedStream(Stream target)
            {
                Target = target;
            }

            public Stream Target { get; }

            public override bool CanRead => Target.CanRead;
            public override bool CanSeek => Target.CanSeek;
            public override bool CanWrite => Target.CanWrite;
            public override long Length => Target.Length;
            public override long Position { get => Target.Position; set => Target.Position = value; }

            public override int Read(byte[] buffer, int offset, int count)
            {
                byte[] original = new byte[count];
                int read = Target.Read(original, 0, count);
                int realCount = Math.Min(read, count);

                for (int i = 0; i < realCount; i++)
                {
                    buffer[offset + i] = (byte)(original[i] ^ ~0);
                }

                return read;
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                byte[] value = new byte[count];

                for (int i = 0; i < count; i++)
                {
                    value[i] = (byte)(buffer[offset + i] ^ ~0);
                }

                Target.Write(value, 0, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return Target.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                Target.SetLength(value);
            }

            public override void Flush()
            {
                Target.Flush();
            }

            protected override void Dispose(bool disposing)
            {
                Target.Dispose();
            }
        }
    }
}