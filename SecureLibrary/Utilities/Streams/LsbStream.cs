using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureLibrary.Utilities.Streams
{
    /// <summary>
    /// A stream that provides a method for storing data in a steganographic LSB image
    /// </summary>
    public class LsbStream : Stream
    {
        private readonly ISpan<byte> Target;
        private long _Position;

        private readonly bool KeepOpen;

        public LsbStream(ISpan<byte> target)
        {
            Target = target;
        }

        public LsbStream(ISpan<byte> target, bool keepOpen)
        {
            Target = target;
            KeepOpen = keepOpen;
        }

        public override bool CanRead => true;
        public override bool CanSeek => true;
        public override bool CanWrite => true;

        public override long Length => Target.Length;

        public override long Position { get => _Position; set => SetPosition(value); }

        public long ActualPosition => Position * 8 % Length;
        public int CurrentBit => (int)(Position * 8 / Length);

        public bool IsDisposed { get; private set; }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - offset < count) throw new ArgumentException(null, nameof(offset));

            int read = 0;
            int end = offset + count;

            int targetPos = (int)ActualPosition;
            int targetBit = CurrentBit;

            for (int i = offset; i < end; i++)
            {
                if (Position >= Length)
                {
                    return read;
                }

                byte readByte = 0;

                for (int byteIndex = 0; byteIndex < 8; byteIndex++)
                {
                    byte targetByte = Target[targetPos++];
                    byte bit = (byte)((targetByte & (1 << targetBit)) >> targetBit);
                    readByte |= (byte)(bit << byteIndex);

                    if (targetPos >= Length)
                    {
                        targetPos = 0;
                        targetBit++;

                        if (targetBit >= 8)
                        {
                            break;
                        }
                    }
                }

                buffer[i] = readByte;
                _Position++;
                read++;

                if (targetBit >= 8)
                {
                    return read;
                }
            }

            return read;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer is null) throw new ArgumentNullException(nameof(buffer));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (buffer.Length - offset < count) throw new ArgumentException(null, nameof(offset));

            int end = offset + count;

            int targetPos = (int)ActualPosition;
            int targetByteIndex = CurrentBit;

            for (int i = offset; i < end; i++)
            {
                if (Position >= Length)
                {
                    throw new IndexOutOfRangeException();
                }

                byte bufferByte = buffer[i];

                for (int byteIndex = 0; byteIndex < 8; byteIndex++)
                {
                    byte targetByte = Target[targetPos];
                    byte bit = (byte)((bufferByte & (1 << byteIndex)) >> byteIndex);
                    byte value = (byte)((targetByte & ~(1 << targetByteIndex)) | (bit << targetByteIndex));
                    Target[targetPos++] = value;

                    if (targetPos >= Length)
                    {
                        targetPos = 0;
                        targetByteIndex++;

                        if (targetByteIndex >= 8)
                        {
                            return;
                        }
                    }
                }

                _Position++;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => Position + offset,
                SeekOrigin.End => Length + offset,
                _ => throw new InvalidOperationException("Wrong SeekOrigin value"),
            };
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("LsbStream does not support setting length");
        }

        private void SetPosition(long value)
        {
            if (value < 0 || value > Length)
            {
                throw new IndexOutOfRangeException();
            }
            _Position = value;
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                if (!KeepOpen && Target is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
