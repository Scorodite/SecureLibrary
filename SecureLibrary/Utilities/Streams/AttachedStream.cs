using System;
using System.IO;
using System.Text;

namespace SecureLibrary.Utilities.Streams
{
    /// <summary>
    /// Writes binary data to the edge of the stream.
    /// When disposed, marks written data with signature so that written data can be detected on next read.
    /// </summary>
    public class AttachedStream : Stream
    {
        private const ulong Signature = 0xA1B2C3D4E5F6A7B8;

        private readonly long _Length;

        public AttachedStream(Stream stream, bool read)
        {
            Source = stream;

            CanRead = read;
            CanWrite = !read;

            if (stream.Length > 16)
            {
                using BinaryReader reader = new(Source, Encoding.UTF8, true);
                Source.Position = Source.Length - 8;
                ulong sign = reader.ReadUInt64();
                if (sign == Signature)
                {
                    Source.Position = Source.Length - 16;
                    long length = reader.ReadInt64();

                    BeginPosition = Source.Position = Source.Length - length - 16;

                    if (read)
                    {
                        _Length = Source.Length - BeginPosition;
                    }

                    return;
                }
            }

            if (read)
            {
                throw new Exception("Signature mismatch");
            }
            else
            {
                BeginPosition = Source.Position = Source.Length;
            }
        }

        public Stream Source { get; }
        public long BeginPosition { get; }

        public override bool CanRead { get; }
        public override bool CanWrite { get; }
        public override long Length => CanRead ? _Length : Source.Position - BeginPosition;

        public override bool CanSeek => false;

        public override long Position
        {
            get => Source.Position - BeginPosition;
            set => throw new NotSupportedException("AttachedStream does not support setting position");
        }

        public override void Flush()
        {
            Source.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (CanRead)
            {
                return Source.Read(buffer, offset, count);
            }
            else
            {
                throw new NotSupportedException("Current AttachedStream mode does not support reading");
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("AttachedStream does not support seeking");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("AttachedStream does not support setting length");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (CanWrite)
            {
                Source.Write(buffer, offset, count);
            }
            else
            {
                throw new NotSupportedException("Current AttachedStream mode does not support writing");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (CanWrite)
                {
                    using BinaryWriter writer = new(Source);
                    writer.Write(Length);
                    writer.Write(Signature);

                    if (Source.Position != Source.Length)
                    {
                        Source.SetLength(Source.Position);
                    }
                }
                Source.Dispose();
            }
        }
    }
}
