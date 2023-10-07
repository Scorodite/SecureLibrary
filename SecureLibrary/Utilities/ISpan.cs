using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureLibrary.Utilities
{
    /// <summary>
    /// Interface of T values storage with fixed length
    /// </summary>
    public interface ISpan<T> : IEnumerable<T>
    {
        public int Length { get; }
        public T this[int index] { get; set; }

        public void CopyTo(ISpan<T> span)
        {
            for (int i = 0; i < Length; i++)
            {
                span[i] = this[i];
            }
        }
    }
}
