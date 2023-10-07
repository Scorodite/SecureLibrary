using SecureLibrary.Core.Storage;

namespace SecureLibrary.ExamplePlugin
{
    public class MemoryStorage : BinaryStorage
    {
        public static MemoryStorage Instance { get; }
        public static MemoryStream Storage { get; }

        public override string Name => "Memory";

        static MemoryStorage()
        {
            Instance = new();
            Storage = new();
        }

        protected MemoryStorage() { }

        public override Stream OpenWrite()
        {
            Storage.Position = 0;
            return new WrapperStream(Storage);
        }

        public override Stream OpenRead()
        {
            Storage.Position = 0;
            return new WrapperStream(Storage);
        }

        /// <summary>
        /// Required for preventing Storage disposing
        /// </summary>
        private class WrapperStream : Stream
        {
            public WrapperStream(Stream target)
            {
                Target = target;
            }

            public Stream Target { get; }

            public override bool CanRead => Target.CanRead;
            public override bool CanSeek => Target.CanSeek;
            public override bool CanWrite => Target.CanWrite;
            public override long Length => Target.Length;
            public override long Position { get => Target.Position; set => Target.Position = value; }

            public override void Flush()
            {
                Target.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return Target.Read(buffer, offset, count);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                Target.Write(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return Target.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                Target.SetLength(value);
            }
        }
    }
}