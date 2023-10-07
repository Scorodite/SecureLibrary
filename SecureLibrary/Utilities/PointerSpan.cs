using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SecureLibrary.Utilities
{
    public readonly struct PointerSpan : ISpan<byte>
    {
        public readonly nint Pointer;
        public readonly int Length;

        public PointerSpan(nint ptr, int length)
        {
            Pointer = ptr;
            Length = length;
        }

        public byte this[int index] { get => Read(index); set => Write(index, value); }

        int ISpan<byte>.Length => Length;

        byte ISpan<byte>.this[int index] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public byte Read(int index)
        {
            if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
            else return Marshal.ReadByte(Pointer, index);
        }

        public void Write(int index, byte value)
        {
            if (index < 0 || index >= Length) throw new IndexOutOfRangeException();
            else Marshal.WriteByte(Pointer, index, value);
        }

        public IEnumerator<byte> GetEnumerator()
        {
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
