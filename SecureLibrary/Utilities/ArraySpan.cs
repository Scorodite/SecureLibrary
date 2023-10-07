using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace SecureLibrary.Utilities
{
    public readonly struct ArraySpan<T> : ISpan<T>
    {
        public ArraySpan(T[] source)
        {
            Source = source;
        }

        public ArraySpan(int length) : this(new T[length]) { }

        public T[] Source { get; }

        public int Length => Source.Length;

        public T this[int index] { get => Source[index]; set => Source[index] = value; }

        public IEnumerator<T> GetEnumerator()
        {
            return Source.Cast<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Source.GetEnumerator();
        }
    }
}
