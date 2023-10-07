using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SecureLibrary.Utilities
{
    /// <summary>
    /// Locks image bits, allows image data to be written and read and unlocks them when disposed
    /// </summary>
    public class ImageLocker : IDisposable, ISpan<byte>
    {
        private readonly Bitmap _Image;
        private readonly BitmapData _ImageData;
        private readonly bool _LeaveOpen;

        public ImageLocker(Bitmap image, ImageLockMode mode, PixelFormat format)
        {
            _Image = image;
            _ImageData = _Image.LockBits(new(0, 0, _Image.Width, _Image.Height), mode, format);
        }

        public ImageLocker(Bitmap image, ImageLockMode mode, PixelFormat format, bool leaveOpen) :
               this(image, mode, format)
        {
            _LeaveOpen = leaveOpen;
        }

        public ImageLocker(Bitmap image, ImageLockMode mode, PixelFormat format, bool leaveOpen,
                           string? fileName) : this(image, mode, format, leaveOpen)
        {
            FileName = fileName;
        }

        public byte this[int index] { get => Read(index); set => Write(index, value); }
        public bool IsDisposed { get; private set; }

        public Bitmap Image
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                else return _Image;
            }
        }

        public BitmapData ImageData
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                else return _ImageData;
            }
        }

        public nint Pointer
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                else return _ImageData.Scan0;
            }
        }

        public int Length
        {
            get
            {
                if (IsDisposed) throw new ObjectDisposedException(ToString());
                else return _ImageData.Stride * _ImageData.Height;
            }
        }

        /// <summary>
        /// If is not null, image will be saved under this file name when ImageLocked disposes
        /// </summary>
        public string? FileName { get; set; }

        public byte Read(int index)
        {
            if (IsDisposed) throw new ObjectDisposedException(ToString());
            else if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
            else return Marshal.ReadByte(_ImageData.Scan0, index);
        }

        public void Write(int index, byte value)
        {
            if (IsDisposed) throw new ObjectDisposedException(ToString());
            else if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
            else Marshal.WriteByte(_ImageData.Scan0, index, value);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            
            if (!IsDisposed)
            {
                IsDisposed = true;
                _Image.UnlockBits(_ImageData);
                
                if (FileName is not null)
                {
                    _Image.Save(FileName);
                }

                if (!_LeaveOpen)
                {
                    _Image.Dispose();
                }
            }
        }

        public IEnumerator<byte> GetEnumerator()
        {
            if (IsDisposed) throw new ObjectDisposedException(ToString());

            for (int i = 0; i < Length; i++)
            {
                yield return Read(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
